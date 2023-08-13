def version = ''
def chartVersion = ''
def chartAction = ''
def patch = ''
def shouldUninstall = ''
def deployAction = ''
def message = ''
pipeline {
    agent any
    options {
         buildDiscarder(logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '', daysToKeepStr: '180', numToKeepStr: '15'))
    }
    stages {
        stage('Prepare') {
            steps {
                sh 'echo "Build Summary: \n" > SUMMARY'
                script {
                    withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                        hangoutsNotifyBuildStart token: "$CHATS_TOKEN",threadByJob: false
                    }
                    
                    // Get some code from a GitHub repository
                    git branch: '$BRANCH_NAME', url: 'https://github.com/Veronneau-Techno-Conseil/CommunAxiomWeb.git'
                
                    version = readFile('VERSION').trim()
                    chartVersion = readFile('./helm/VERSION').trim()
                    patch = version.trim()
                    buildEnvImage = 'vertechcon/comax-buildenv:1.0.1'
                }
                echo "$buildEnvImage"
            }
        }
        stage('Node compile') {
            steps {
                nodejs(nodeJSInstallationName: 'NodeJs') {
                    sh 'cd "src/CommunAxiomWeb/" && npm install'
                    sh 'cd "src/CommunAxiomWeb/" && node ./browserify.js'
                    sh 'cd "src/CommunAxiomWeb/" && npx gulp min'
                }
            }
        }
        stage('Build') {
            steps {
                withCredentials([[$class: 'UsernamePasswordMultiBinding', credentialsId:'dockerhub_creds', usernameVariable: 'USERNAME', passwordVariable: 'PASSWORD']]) {
                    sh 'if [ -z "$(docker buildx ls | grep multiarch)" ]; then docker buildx create --name multiarch --driver docker-container --use; else docker buildx use multiarch; fi'
                    sh "docker login -u ${USERNAME} -p ${PASSWORD}"
                    sh "docker buildx build --push -t vertechcon/comaxweb:latest -t vertechcon/comaxweb:${patch} --platform linux/amd64,linux/arm64 -f Dockerfile ."
                    sh 'echo "Build vertechcon/comaxweb:${version} pushed to registry \n" >> SUMMARY'
                }
            }

            post {
                success {
                    script {
                        currentBuild.displayName = version
                    }
                }                
            }
        }
        stage('Prep Helm') {
            agent {
                docker {
                    image "$buildEnvImage"
                    reuseNode true
                }
            }
            steps {
                sh 'mkdir penv && python3 -m venv ./penv'
                sh '. penv/bin/activate && pwd && ls -l && pip install -r ./build/requirements.txt && python3 ./build/processchart.py'
                sh 'res=$(curl -k "https://charts.vtck3s.lan/api/charts/comax-web/0.1.10" )&& echo $res | jq \'if .name != null then .name else "DEPLOY" end\' > CHART_ACTION'
                script {
                    chartAction = readFile('CHART_ACTION').replace('"','').trim()
                }
            }
        }
        stage('Helm') {
            agent {
                docker {
                    image "$buildEnvImage"
                    reuseNode true
                }
            }
            when{
                expression {
                    return chartAction == "DEPLOY"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm lint ./helm/'
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm/ --repository-config ${repos}'
                    sh 'helm package ./helm/ --repository-config ${repos}'
                    sh 'CHARTVER=$(cat ./helm/VERSION) && curl -k --data-binary "@comax-web-$CHARTVER.tgz" https://charts.vtck3s.lan/api/charts'
                }
            }
        }
        stage('SKIP Helm') {
            when{
                expression {
                    return chartAction != "DEPLOY"
                }
            }
            steps {
                sh 'echo "Skipped helm chart deployment du to preexisting chart version ${chartVersion} \n" >> SUMMARY'
            }
        }
        stage('Prepare Application deployment') {
            agent {
                docker {
                    image "$buildEnvImage"
                    reuseNode true
                }
            }
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release')
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm --repository-config ${repos}'
                    sh 'helm list -n comaxws --output=json --kubeconfig $kubecfg > HELM_LIST'
                    sh 'cat HELM_LIST'
                    sh 'jq \'.[] | select(.name == "comaxweb") | select(.status == "deployed") | "upgrade"\' HELM_LIST > DEPLOY_ACTION'
                    sh 'jq \'.[] | select(.name == "comaxweb") | select(.status != "deployed") | "uninstall"\' HELM_LIST > SHOULD_UNINSTALL'
                    sh 'cat DEPLOY_ACTION && cat SHOULD_UNINSTALL'
                    script {
                        deployAction = readFile('DEPLOY_ACTION').replace('"','').trim()
                        shouldUninstall = readFile('SHOULD_UNINSTALL').replace('"','').trim()
                    }
                    echo "Deploy action: ${deployAction}"
                    echo "Should uninstall: ${shouldUninstall}"
                }
            }
        }
        stage('Uninstall Application deployment') {
            agent {
                docker {
                    image "$buildEnvImage"
                    reuseNode true
                }
            }
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && shouldUninstall == 'uninstall'
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm -n comaxws uninstall comaxweb --kubeconfig ${kubecfg}'
                }
            }
        }
        stage('Install Application deployment') {
            agent {
                docker {
                    image "$buildEnvImage"
                    reuseNode true
                }
            }
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction != "upgrade"
                }
            }
            steps {
                echo "Deploy action: ${deployAction}"
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'comax_web_values', variable: 'comax_web_values')]) {
                    sh 'helm -n comaxws install comaxweb ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${comax_web_values}'
                }
            }
        }
        stage('Upgrade Application deployment') {
            agent {
                docker {
                    image "$buildEnvImage"
                    reuseNode true
                }
            }
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction == "upgrade"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'comax_web_values', variable: 'comax_web_values')]) {
                    sh 'helm -n comaxws upgrade comaxweb ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${comax_web_values}'
                }
            }
        }
        stage('Finalize') {
            agent {
                docker {
                    image "$buildEnvImage"
                    reuseNode true
                }
            }
            steps {
                script {
                    message = readFile('SUMMARY')
                }
            }
        }
    }
    post {
        success {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotify message: message, token: "$CHATS_TOKEN", threadByJob: false
                hangoutsNotifySuccess token: "$CHATS_TOKEN", threadByJob: false
            }
        }
        failure {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotify message: message, token: "$CHATS_TOKEN", threadByJob: false
                hangoutsNotifyFailure token: "$CHATS_TOKEN", threadByJob: false
            }
        }
        always {
            script {
                cleanWs()
            }
        }
    }
}

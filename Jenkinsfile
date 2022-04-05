def version = ''
def chartVersion = ''
def chartAction = ''
def patch = ''
def shouldUninstall = ''
def deployAction = ''
def message = ''
pipeline {
    agent any

    stages {
        stage('Prepare') {
            steps {
                sh 'echo "Build Summary: \n" > summary'
                script {
                    withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                        hangoutsNotifyBuildStart token: "$CHATS_TOKEN",threadByJob: false
                    }
                    
                    // Get some code from a GitHub repository
                    git branch: '$BRANCH_NAME', url: 'https://github.com/Veronneau-Techno-Conseil/CommunAxiomWeb.git'
                
                    version = readFile('VERSION')
                    chartVersion = readFile('./helm/VERSION')
                    patch = version.trim()
                }
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
                script {
                    def customImage = docker.build("registry.vtck3s.lan/comaxweb:latest")
                    customImage.push()
                    customImage.push(patch)
                }
                sh 'echo "Build registry.vtck3s.lan/comaxweb:${version} pushed to registry \n" >> summary'
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
            steps {
                sh 'mkdir penv && python3 -m venv ./penv'
                sh '. penv/bin/activate && pwd && ls -l && pip install -r ./build/requirements.txt && python3 ./build/processchart.py'
                sh 'curl -k https://charts.vtck3s.lan/api/charts/comax-web/${chartVersion} | jq \'.name | "DEPLOY"\' > CHART_ACTION'
                script {
                    chartAction = readFile('CHART_ACTION').replace('"','')
                }
            }
        }
        stage('Helm') {
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
                sh 'echo "Skipped helm chart deployment du to preexisting chart version ${chartVersion} \n" >> summary'
            }
        }
        stage('Prepare Application deployment') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release')
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm --repository-config ${repos}'
                    sh 'helm -n comaxws list --output=json --kubeconfig ${kubecfg} > helmlist'
                    sh 'jq \'select(.[].name == "comaxweb") | select(.[].status == "deployed") | "upgrade" \' helmlist > deployAction'
                    sh 'jq \'select(.[].name == "comaxweb") | select(.[].status != "deployed") | "uninstall" \' helmlist > shouldUninstall'
                    script {
                        deployAction = readFile('deployAction').replace('"','')
                        shouldUninstall = readFile('shouldUninstall').replace('"','')
                    }
                    sh 'echo "Deploy action: ${deployAction}"'
                    sh 'echo "Should uninstall: ${shouldUninstall}"'
                }
            }
        }
        stage('Uninstall Application deployment') {
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
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction != "upgrade"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm -n comaxws install comaxweb ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos}'
                }
            }
        }
        stage('Upgrade Application deployment') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction == "upgrade"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm -n comaxws upgrade comaxweb ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos}'
                }
            }
        }
        stage('Finalize') {
            steps {
                script {
                    message = readFile('summary')
                }
            }
        }
    }
    post {
        success {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotify message: message, token: '${CHATS_TOKEN}', threadByJob: false
                hangoutsNotifySuccess token: '${CHATS_TOKEN}', threadByJob: false
            }
        }
        failure {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotify message: message, token: '${CHATS_TOKEN}', threadByJob: false
                hangoutsNotifyFailure token: '${CHATS_TOKEN}',threadByJob: false
            }
        }
        always {
            script {
                cleanWs()
            }
        }
    }
}

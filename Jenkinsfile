def version = ''
def patch = ''

pipeline {
    agent any

    stages {
        stage('Prepare') {
            steps {
                script {
                    withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                        hangoutsNotifyBuildStart token: "$CHATS_TOKEN",threadByJob: false
                    }
                    
                    // Get some code from a GitHub repository
                    git branch: 'main', url: 'https://github.com/Veronneau-Techno-Conseil/CommunAxiomWeb.git'
                
                    version = readFile('VERSION')
                    //def versions = version.split('\\.')
                    //def major = versions[0]
                    //def minor = versions[0] + '.' + versions[1]
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
            }

            post {
                success {
                    script {
                        currentBuild.displayName = version
                        //buildDescription("Committer: ${GERRIT_PATCHSET_UPLOADER_NAME}")
                    }
                }                
            }
        }
        stage('Prep Helm') {
            steps {
                sh 'mkdir penv && python3 -m venv ./penv'
                sh '. penv/bin/activate && pwd && ls -l && pip install -r ./build/requirements.txt && python3 ./build/processchart.py'
            }
        }
        stage('Helm') {
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm lint ./helm/'
                    sh 'helm package ./helm/'
                    sh 'CHARTVER=$(cat ./helm/VERSION) && curl -k --data-binary "@comax-web-$CHARTVER.tgz" https://charts.vtck3s.lan/api/charts'
                }
            }
        }
    }
    post {
        success {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotifySuccess token: "$CHATS_TOKEN",threadByJob: false
            }
        }
        failure {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotifyFailure token: "$CHATS_TOKEN",threadByJob: false
            }
        }
        always {
            script {
                cleanWs()
            }
        }
    }
}

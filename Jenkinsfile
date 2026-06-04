pipeline {
    agent any 

    stages {
        stage('Build') {
            steps {
                echo 'Building the application...'
                // sh 'npm run build' or 'mvn clean package'
            }
        }
        
        stage('Test') {
            steps {
                echo 'Running automated tests...'
                // sh 'npm test' or 'mvn test'
            }
        }
        
        stage('Deploy') {
            steps {
                echo 'Deploying to the server...'
                // sh './deploy.sh'
            }
        }
    }

    post {
        always {
            echo 'This will always run, cleaning up resources...'
        }
        success {
            echo 'Pipeline completed successfully!'
        }
        failure {
            echo 'Pipeline failed. Sending alerts...'
        }
    }
}

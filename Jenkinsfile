pipeline {
  agent {
	node {
	  label 'windows_dotnetcore_preview2'
	  customWorkspace "\\${env.BRANCH_NAME}"
	}
  }
  environment {
	NUGET_URL = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
  }
  stages {
	stage("Cleanup") {
	  steps {
		// Delete old build files
		bat "powershell \"Get-ChildItem .\\ -include bin,obj,artifacts -Recurse | foreach (\$_) { remove-item \$_.fullname -Force -Recurse }\" || exit 0"
	  }
	}
	stage("Build") {
	  steps {
		// Restore
		bat "dotnet restore"
		
		// Build
		bat "dotnet build src\\GraphQL-Linq --configuration release --version-suffix beta${env.BUILD_ID}"
		
		// Tests
		bat "dotnet build tests\\GraphQL-Linq.Tests --configuration release --version-suffix beta${env.BUILD_ID}"
	  }
	}
	stage("Tests") {
	  steps {
		// Run tests
		bat "dotnet test tests\\GraphQL-Linq.Tests --configuration release -xml Tests.xml"
		step([$class: 'XUnitBuilder', testTimeMargin: '3000', thresholdMode: 2, thresholds: [[$class: 'FailedThreshold', failureNewThreshold: '', failureThreshold: '', unstableNewThreshold: '', unstableThreshold: ''], [$class: 'SkippedThreshold', failureNewThreshold: '', failureThreshold: '', unstableNewThreshold: '', unstableThreshold: '']], tools: [[$class: 'XUnitDotNetTestType', deleteOutputFiles: true, failIfNotNew: true, pattern: 'Tests.xml', skipNoTestFiles: false, stopProcessingIfError: true]]])
	  }
	}
	stage("Nuget") {
	  steps {
		// Pack Nuget
		bat "dotnet pack src\\GraphQL-Linq --configuration release --output artifacts --version-suffix beta${env.BUILD_ID}"
		archiveArtifacts artifacts: '**/*.nupkg', fingerprint: true
		
		// Publish Nuget
		bat "powershell wget %NUGET_URL% -OutFile nuget.exe"
		withCredentials([[$class: 'UsernamePasswordMultiBinding', credentialsId: 'nuget.sahbdev.dk',
			usernameVariable: 'username', passwordVariable: 'password']]) {
		  bat "nuget.exe push artifacts\\*beta${env.BUILD_ID}.nupkg %password% -Source https://nuget.sahbdev.dk/api/v2/package"
		}
	  }
	}
  }
}
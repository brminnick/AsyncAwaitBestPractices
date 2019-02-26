#!/usr/bin/env bash
 2>&1

UITestProject=`find "$BuildPath" -name HackerNews.UITests.csproj`
echo UITestProject: $UITestProject

UITestDLL=`find "$BuildPath" -name "HackerNews.UITests.dll" | grep bin`
echo UITestDLL: $UITestDLL

UITestBuildDir=`dirname $UITestDLL`
echo UITestBuildDir: $UITestBuildDir

UITestVersionNumber=`grep '[0-9]' $UITestProject | grep Xamarin.UITest|grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,10\}\-'dev`
echo UITestPrereleaseVersionNumber: $UITestVersionNumber

UITestVersionNumberSize=${#UITestVersionNumber} 
echo UITestVersionNumberSize: $UITestVersionNumberSize

if [[ $UITestVersionNumberSize == 0 ]];
then
    UITestVersionNumber=`grep '[0-9]' $UITestProject | grep Xamarin.UITest|grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}'`
    echo UITestVersionNumber: $UITestVersionNumber
fi

TestCloudExe=`find "$NuGetPackagesPath" | grep test-cloud.exe | grep $UITestVersionNumber | head -1`
echo TestCloudExe: $TestCloudExe

TestCloudExeDirectory=`dirname $TestCloudExe`
echo TestCloudExeDirectory: $TestCloudExeDirectory

APKFile=`find "$BuildPath" -name *.apk | head -1`
echo APKFile: $APKFile

npm install -g appcenter-cli

appcenter login --token $1

appcenter test run uitest --app "AsyncAwaitBestPractices/AsyncAwaitBestPractices.Droid" --devices "AsyncAwaitBestPractices/all-devices" --app-path $APKFile --test-series "master" --locale "en_US" --build-dir $UITestBuildDir --uitest-tools-dir $TestCloudExeDirectory
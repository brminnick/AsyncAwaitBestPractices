#!/usr/bin/env bash

set -e

UITestProject=`find "$BuildPath" -name HackerNews.UITests.csproj` 2>&1
echo UITestProject: $UITestProject

UITestDLL=`find "$BuildPath" -name "HackerNews.UITests.dll" | grep bin` 2>&1
echo UITestDLL: $UITestDLL

UITestBuildDir=`dirname $UITestDLL` 2>&1
echo UITestBuildDir: $UITestBuildDir

UITestVersionNumber=`grep '[0-9]' $UITestProject | grep Xamarin.UITest|grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,10\}\-'dev` 2>&1
echo UITestPrereleaseVersionNumber: $UITestVersionNumber

UITestVersionNumberSize=${#UITestVersionNumber} 2>&1
echo UITestVersionNumberSize: $UITestVersionNumberSize

if [[ $UITestVersionNumberSize == 0 ]];
then
    UITestVersionNumber=`grep '[0-9]' $UITestProject | grep Xamarin.UITest|grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}'`
    echo UITestVersionNumber: $UITestVersionNumber
fi

TestCloudExe=`find "$NuGetPackagesPath" | grep test-cloud.exe | grep $UITestVersionNumber | head -1` 2>&1
echo TestCloudExe: $TestCloudExe

TestCloudExeDirectory=`dirname $TestCloudExe` 2>&1
echo TestCloudExeDirectory: $TestCloudExeDirectory

APKFile=`find "$BuildPath" -name *.apk | head -1` 2>&1
echo APKFile: $APKFile

npm install -g appcenter-cli 2>&1

appcenter login --token $1 2>&1

appcenter test run uitest --app "AsyncAwaitBestPractices/AsyncAwaitBestPractices.Droid" --devices "AsyncAwaitBestPractices/all-devices" --app-path $APKFile --test-series "master" --locale "en_US" --build-dir $UITestBuildDir --uitest-tools-dir $TestCloudExeDirectory 2>&1
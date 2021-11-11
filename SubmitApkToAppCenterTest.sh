#!/usr/bin/env bash

set -e

UITestProject=`find "$BuildPath" -name HackerNews.UITests.csproj`
echo UITestProject: $UITestProject

msbuild $UITestProject /t:Restore

UITestDLL=`find "$BuildPath" -name "HackerNews.UITests.dll" | grep bin | grep -v ref | head -1`
echo UITestDLL: $UITestDLL

UITestBuildDir=`dirname $UITestDLL`
echo UITestBuildDir: $UITestBuildDir

UITestVersionNumber=`grep '[0-9]' $UITestProject | grep Xamarin.UITest|grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}'`
echo UITestVersionNumber: $UITestVersionNumber

TestCloudExe=`find ~/.nuget | grep test-cloud.exe | grep $UITestVersionNumber | head -1`
echo TestCloudExe: $TestCloudExe

TestCloudExeDirectory=`dirname $TestCloudExe`
echo TestCloudExeDirectory: $TestCloudExeDirectory

APKFile=`find "$BuildPath" -name *.apk | head -1` 
echo APKFile: $APKFile

npm install -g appcenter-cli@1.2.2 2>&1

appcenter login --token $1

appcenter test run uitest --app "CDA-Global-Beta/AsyncAwaitBestPractices.Droid" --devices "CDA-Global-Beta/android-os-v5-10" --app-path $APKFile --test-series "master" --locale "en_US" --build-dir $UITestBuildDir --uitest-tools-dir $TestCloudExeDirectory --async
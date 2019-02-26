#!/usr/bin/env bash
UITestProject=`find "D:\a\1\s" -name HackerNews.UITests.csproj`
echo UITestProject: $UITestProject

UITestVersionNumber=`grep '[0-9]' $UITestProject | grep Xamarin.UITest | grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,10\}\-'dev`
echo UITestPrereleaseVersionNumber: $UITestVersionNumber

UITestDLL=`find "D:\a\1\s"  -name "HackerNews.UITests.dll" | grep bin`
echo UITestDLL: $UITestDLL

UITestBuildDir=`dirname $UITestDLL`
echo UITestBuildDir: $UITestBuildDir

##vso[task.setvariable variable=UITestBuildDir;isSecret=false;isOutput=true;]$UITestBuildDir

UITestVersionNumberSize=${#UITestVersionNumber} 
echo UITestVersionNumberSize: $UITestVersionNumberSize

if [[ $UITestDevVersionNumberSize == 0 ]];
then
    UITestVersionNumber=`grep '[0-9]' $UITestProject | grep Xamarin.UITest | grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}'`
    echo UITestVersionNumber: $UITestVersionNumber
fi

##vso[task.setvariable variable=UITestVersionNumber;isSecret=false;isOutput=true;]$UITestVersionNumber
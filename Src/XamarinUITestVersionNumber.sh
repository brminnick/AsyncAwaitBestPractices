#!/usr/bin/env bash
set UITestProject=`find "%Agent.BuildDirectory%" -name HackerNews.UITests.csproj`
echo UITestProject: %UITestProject%

set UITestDLL=`find "%Agent.BuildDirectory%" -name "HackerNews.UITests.dll" | grep bin`
echo UITestDLL: %UITestDLL%

set UITestBuildDir=`dirname %UITestDLL%`
echo UITestBuildDir: %UITestBuildDir%

UITestVersionNumber=`grep '[0-9]' %UITestProject% | grep Xamarin.UITest | grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,10\}\-'dev`
echo UITestPrereleaseVersionNumber: $UITestVersionNumber

UITestVersionNumberSize=${#UITestVersionNumber} 
echo UITestVersionNumberSize: $UITestVersionNumberSize

if [ $UITestDevVersionNumberSize == 0 ]
then
    UITestVersionNumber=`grep '[0-9]' %UITestProject% | grep Xamarin.UITest|grep -o '[0-9]\{1,3\}\.[0-9]\{1,3\}\.[0-9]\{1,3\}'`
    echo UITestVersionNumber: $UITestVersionNumber
fi

set UITestVersionNumber = $UITestVersionNumber
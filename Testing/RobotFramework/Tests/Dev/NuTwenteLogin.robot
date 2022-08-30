*** Settings ***
Documentation       Testing NuTwenteWeb
Resource            ../../Data/InputData.robot
Resource            ../../Resources/Common/Common.robot
Resource            ../../Resources/NuTwente/NuTwenteApp.robot
Test Setup          Common.Begin web test
Test Teardown       Common.End web test

*** Variables ***
${BROWSER}      chrome
${LOGIN_URL}    http://nutwente-dev.azurewebsites.net/

*** Test Cases ***
Should be able to acces login page
    [Documentation]    vertify login page is loading
    [Tags]  Smoke
    NuTwenteApp.Should be able to acces Login page

Login to existing account 1
    [Documentation]    vertify loggin into accout 1 is working
    [Tags]  Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_1}

Login to existing account 2
    [Documentation]    vertify loggin into accout 2 is working
    [Tags]  Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_2}

Login to existing account 3
    [Documentation]    vertify loggin into accout 3 is working
    [Tags]  Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_3}

Login to existing account 4
    [Documentation]    vertify loggin into accout 4 is working
    [Tags]  Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_4}

#Should be able to change personal information
#    [Documentation]    vertify its possible to change personal information
#    [Tags]  Functional
#    NuTwenteApp.Should be able to acces Login page
#    Should be able to login     &{USER_1}

#Simply tests run
#robot -d results tests/NuTwenteLogin.robot
#robot -d results tests/NuTwente/NuTwenteLogin.robot

#Complex tests run (-L Trace gives even more info)
#robot -d results -L debug --reporttitle "RobotFramework Report" --logtitle "RobotFramework Log" --timestampoutputs tests/NuTwenteLogin.robot

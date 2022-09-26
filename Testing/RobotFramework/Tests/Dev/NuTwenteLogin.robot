*** Settings ***
Library             String
Documentation       Testing NuTwenteWeb
Resource            ../../Data/InputData.robot
Resource            ../../Resources/Common/Common.robot
Resource            ../../Resources/NuTwente/NuTwenteApp.robot
Test Setup          Common.Begin web test
Test Teardown       Common.End web test

*** Variables ***
${BROWSER}              Chrome
${LOGIN_URL}            http://nutwente-dev.azurewebsites.net/

*** Test Cases ***
Should be able to acces login page
    [Documentation]     Vertify login page is loading
    [Tags]              Smoke
    NuTwenteApp.Should be able to acces Login page

Should be able to change personal information account 1
    [Documentation]     Vertify its possible to change personal information
    [Tags]              Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_1}

    ${Random}   Generate Random String  1   [NUMBERS]

    nutwenteapp.should be able to change profile data   &{profiles}[${Random}]
    #${profiles}[numbers][0]
    #&{profiles}[0]
    #&{Profiel_1}

Should be able to change personal information account 2
    [Documentation]     Vertify its possible to change personal information
    [Tags]              Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_2}

    ${Random}   Generate Random String  1   [NUMBERS]

    nutwenteapp.should be able to change profile data   &{profiles}[${Random}]
    #&{Profiel_2}

Should be able to change personal information account 3
    [Documentation]     Vertify its possible to change personal information
    [Tags]              Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_3}

    ${Random}   Generate Random String  1   [NUMBERS]

    nutwenteapp.should be able to change profile data   &{profiles}[${Random}]
    #&{profiles}[@{numbers}[0]]
    #&{Profiel_3}

Should be able to change personal information account 4
    [Documentation]     Vertify its possible to change personal information
    [Tags]              Functional
    NuTwenteApp.Should be able to acces Login page
    Should be able to login     &{USER_4}

    ${Random}   Generate Random String  1   [NUMBERS]

    nutwenteapp.should be able to change profile data   &{profiles}[${Random}]

#Login to existing account 1
#    [Documentation]    vertify loggin into accout 1 is working
#    [Tags]  Functional
#    NuTwenteApp.Should be able to acces Login page
#    Should be able to login     &{USER_1}

#Login to existing account 2
#    [Documentation]    vertify loggin into accout 2 is working
#    [Tags]  Functional
#    NuTwenteApp.Should be able to acces Login page
#    Should be able to login     &{USER_2}

#Login to existing account 3
#    [Documentation]    vertify loggin into accout 3 is working
#    [Tags]  Functional
#    NuTwenteApp.Should be able to acces Login page
#    Should be able to login     &{USER_3}

#Login to existing account 4
#    [Documentation]    vertify loggin into accout 4 is working
#    [Tags]  Functional
#    NuTwenteApp.Should be able to acces Login page
#    Should be able to login     &{USER_4}


#Simply tests run
#robot -d results tests/NuTwenteLogin.robot
#robot -d results tests/NuTwente/NuTwenteLogin.robot

#Complex tests run (-L Trace gives even more info)
#robot -d results -L debug --reporttitle "RobotFramework Report" --logtitle "RobotFramework Log" --timestampoutputs tests/NuTwenteLogin.robot

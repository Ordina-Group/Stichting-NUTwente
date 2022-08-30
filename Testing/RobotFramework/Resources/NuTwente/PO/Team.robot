*** Settings ***
Library    SeleniumLibrary

*** Variables ***


*** Keywords ***
VerifyTeamPageLoaded
    wait until page contains    Kay Garland
    log    Landing page test case 1
    sleep    1s
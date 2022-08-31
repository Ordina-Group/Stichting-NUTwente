*** Settings ***
#Resource    ./PO/FrontOffice.LandingPage.robot
Resource    ./PO/NuTwente.LoginPage.robot
Resource    ./PO/Team.robot
Resource    ./PO/TopNav.robot

*** Keywords ***
Should be able to acces Login page
    NuTwente.LoginPage.GoToLandingPage
    NuTwente.LoginPage.VerifyLandingPageLoaded

Should be able to login
    [Arguments]     &{USER}
    NuTwente.LoginPage.Login    &{USER}
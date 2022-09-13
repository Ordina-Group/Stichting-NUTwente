*** Settings ***
#Resource    ./PO/FrontOffice.LandingPage.robot
Resource    ./PO/NuTwente.LoginPage.robot
Resource    ./PO/TopNav.robot
Resource    ./PO/ProfielWijzigen.robot

*** Keywords ***
Should be able to acces Login page
    NuTwente.LoginPage.GoToLandingPage
    NuTwente.LoginPage.VerifyLandingPageLoaded

Should be able to login
    [Arguments]     &{USER}
    NuTwente.LoginPage.Login    &{USER}

Should be able to change profile data
    [Arguments]     &{Profiel}
    topnav.gotoprofielveranderenpagina
    ProfielWijzigen.ShouldBeAbleToChangeProfileData     &{Profiel}

Should be able to change profile data
    [Arguments]     &{Profiel}
    topnav.gotoprofielveranderenpagina
    ProfielWijzigen.ShouldBeAbleToChangeProfileData     &{Profiel}

Should be able to change profile data
    [Arguments]     &{Profiel}
    topnav.gotoprofielveranderenpagina
    ProfielWijzigen.ShouldBeAbleToChangeProfileData     &{Profiel}

Should be able to change profile data
    [Arguments]     &{Profiel}
    topnav.gotoprofielveranderenpagina
    ProfielWijzigen.ShouldBeAbleToChangeProfileData     &{Profiel}
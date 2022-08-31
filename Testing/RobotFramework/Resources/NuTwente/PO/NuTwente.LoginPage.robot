*** Settings ***
Library    SeleniumLibrary

*** Variables ***


*** Keywords ***
GoToLandingPage
    go to   ${LOGIN_URL}

VerifyLandingPageLoaded
    wait until page contains    Sign in with your email address
    log    Login page vertification test

Login
   [Arguments]  &{USER}
   #BuiltIn.sleep    2
   Input Text   id=signInName   ${USER.ID}

   #BuiltIn.sleep    2
   Input Text   id=password     ${USER.password}

   #BuiltIn.sleep    2
   click button    id=next
   BuiltIn.sleep    5

   wait until page contains    Mijn Overzichten
   BuiltIn.sleep    2
*** Settings ***
Library    SeleniumLibrary

*** Variables ***


*** Keywords ***
ShouldBeAbleToChangeProfileData
   [Arguments]  &{Profiel}

    BuiltIn.sleep    2

    Input Text   id=givenName   ${Profiel.Voornaam}
    log     Selecteer voornaam veld

    Input Text   id=surname   ${Profiel.Achternaam}
    log     Selecteer achternaam veld

    Input Text   id=telephoneNumber   ${Profiel.Telefoonnummer}
    log     Selecteer telefoonnummer veld

    BuiltIn.sleep    2

    click button    id=continue
    log     Click on continue button


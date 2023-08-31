*** Settings ***
Library    SeleniumLibrary

*** Keywords ***
GoToProfielVeranderenPagina
    BuiltIn.sleep    2
    click element    xpath=//ul[2]/li/a
    log    Open op hamburger menu that provides acces to profile page

    BuiltIn.sleep    2
    click element   //a[contains(text(),'Profiel wijzigen')]

    BuiltIn.sleep    4
    log    Klik op profiel wijzigen

    wait until page contains    Cancel
    log    check of er succesvol naar de profiel veranderen pagina is genavigeerd
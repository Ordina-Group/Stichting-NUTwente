*** Settings ***
Library    SeleniumLibrary

*** Variables ***


*** Keywords ***
GoToTeamsPage
    click link    Team
    #click element    css=#bs-example-navbar-collapse-1 > ul > li:nth-child(5) > a
    log    click button to go to "team" pager

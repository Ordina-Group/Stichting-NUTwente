*** Settings ***
Library    SeleniumLibrary

*** Variables ***


*** Keywords ***
Begin web test
    open browser    ${START_URL}   ${BROWSER}
    maximize browser window
    sleep   1s

End web test
    close all browsers
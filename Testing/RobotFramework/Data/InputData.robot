*** Variables ***
${START_URL}        about:blank

#@{Profiles}     &{Profiel_1}    &{Profiel_2}    &{Profiel_3}    &{Profiel_4}    &{Profiel_5}    &{Profiel_6}    &{Profiel_7}    &{Profiel_8}    &{Profiel_9}    &{Profiel_10}    &{Profiel_11}    &{Profiel_12}
&{Profiles}       0=&{Profiel_0}   1=&{Profiel_1}    2=&{Profiel_2}    3=&{Profiel_3}    4=&{Profiel_4}  5=&{Profiel_5}    6=&{Profiel_6}    7=&{Profiel_7}    8=&{Profiel_8}    9=&{Profiel_9}

&{Profiel_0}      Voornaam=Niek             Achternaam=Nieuwenhuisen    Telefoonnummer=060123456789
&{Profiel_1}      Voornaam=Vrijwilliger     Achternaam=Test             Telefoonnummer=060123456789
&{Profiel_2}      Voornaam=Admin            Achternaam=Test             Telefoonnummer=060123456789
&{Profiel_3}      Voornaam=secretariaat     Achternaam=Test             Telefoonnummer=060123456789
&{Profiel_4}      Voornaam=coordinator      Achternaam=Test             Telefoonnummer=060123456789

&{Profiel_5}      Voornaam=Tom              Achternaam=Doornbos         Telefoonnummer=060123456789
&{Profiel_6}      Voornaam=Casper           Achternaam=Hooft            Telefoonnummer=060123456789
&{Profiel_7}      Voornaam=Stef             Achternaam=Joosten          Telefoonnummer=060123456789
&{Profiel_8}      Voornaam=Han              Achternaam=Joosten          Telefoonnummer=060123456789
&{Profiel_9}      Voornaam=Larso            Achternaam=Hoog             Telefoonnummer=060123456789

#Uncomment this to enable GitHub secrets
&{USER_1}       ID=NuTwente1@gmail.com    password=${PASSWORD_1}
&{USER_2}       ID=NuTwente2@gmail.com    password=${PASSWORD_2}
&{USER_3}       ID=NuTwente3@gmail.com    password=${PASSWORD_3}
&{USER_4}       ID=NuTwente4@gmail.com    password=${PASSWORD_4}


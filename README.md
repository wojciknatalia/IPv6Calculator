Kalkulator podsieci IPv6 działający podobnie jak program ipcalc dostępny w linuksie. Wejściem programu jest podsieć IPv6 podana wraz z maską, wyjściem programu są informacje takie jak: adres podsieci znormalizowany względem maski, maska sieci, najmniejszy i największy adres w tej podsieci (network range), ilość adresów dostępnych wewnątrz sieci (nie obowiązuje tutaj takie ograniczenie jak w IPv4, że pierwszy i ostatni adres są nieużywalne). Pozostałe informacje jak w przykładowym narzędziu dostępnym tutaj:

http://www.gestioip.net/cgi-bin/subnet_calculator.cgi

W przypadku prezentowania podsieci IPv6 należy także oddzielić spacją cyfrę, która się zmienia względem sąsiednich podsieci (przykład poniżej) oraz zaprezentować dwie sąsiednie podsieci względem podanej maski (poprzednią i następną), przykład:

2a01:1d8:2:280::/58
powinno dać w wyniku:

2a01:1d8:2:2 4 0::/58

2a01:1d8:2:2 8 0::/58

2a01:1d8:2:2 c 0::/58
W wyniku powinna być także prezentacja adresu pełnego (z rozwinięciem wszystkich nieznaczących zer) i skróconego (ze zwinięciem zer), w przypadku niejednoznaczności proszę wybrać krótszy adres. Np:

1:0:0:1:0:0:0:1 powinno być zwinięte do 1:0:0:1::1, nie do 1::1:0:0:0:1

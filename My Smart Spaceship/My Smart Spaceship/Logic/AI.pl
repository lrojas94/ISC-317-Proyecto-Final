% Reglas y hechos para manejar la logica de la inteligancia artificial

% Las reglas "peligroso" definen objetos que son dañinas para la ia o el humano.
% Las reglas "util" se refieren a powerups que benefician al usuario del objeto.
% Las reglas "arma" indican que un powerup sirve para dañar al humano en manos
% del ia o viceversa.

:- dynamic mejora/2.
:- dynamic destruible/1.
:- dynamic vulnerable/2.
:- dynamic invulnerable/2.

acercar(Objeto):- vulnerable(Objeto, ia), !, fail.
acercar(Objeto):- not(util(Objeto, ia)), !, fail.
acercar(_):- !.

alejar(Objeto):- not(vulnerable(Objeto, ia)), !, fail.
alejar(Objeto):- util(Objeto, ia), !, fail.
alejar(_):- !.

disparar(Objeto):- not(destruible(Objeto)), !, fail.
disparar(Objeto):- invulnerable(disparo(ia), Objeto), !, fail.
disparar(Objeto):- util(Objeto, ia), !, fail.
disparar(_):- !.

% La regla mover toma una lista de objetos que estan cerca de la IA
% (la cercania) se decide en el código en C#.
mover([], []):- !.
mover([], _):- !.
mover([Objeto|ListaObjetosCercanos], Acciones):-
		alejar(Objeto), mover(ListaObjetosCercanos, L), Acciones=[alejar|L],!.
mover([Objeto|ListaObjetosCercanos], Acciones):-
		acercar(Objeto), mover(ListaObjetosCercanos, L), Acciones=[acercar|L],!.
mover([_|ListaObjetosCercanos], Acciones):-
		mover(ListaObjetosCercanos, L), Acciones = [quedarse_quieto|L], !.

% Objetos considerados como upgrades de artilleria.
arma(Objeto, ia):- mejora(Objeto, armamento_ia), !.
arma(Objeto, humano):- mejora(Objeto, armamento_humano), !.

% Todas las consideraciones de si algo es util para la IA.
util(Objeto, ia):- vulnerable(Objeto, ia), !, fail.
util(Objeto, ia):- arma(Objeto, humano), !, fail.
util(Objeto, ia):- mejora(Objeto, ia), !.
util(Objeto, ia):- arma(Objeto, ia), !.
util(_):- !.

% uso general: aprendizaje( evento(causa, consecuencia) ).

% Cosas que son vulnerables a que cosas: Naves a disparos, asteroides, etc ...
aprendizaje( evento(impacta(Estimulo, Objeto), perjudica(Estimulo, Objeto)) ):-
		mejora(Estimulo, Objeto), retract(mejora(Estimulo, Objeto)), fail.
aprendizaje( evento(impacta(Estimulo, Objeto), perjudica(Estimulo, Objeto)) ):-
		invulnerable(Estimulo, Objeto), retract(invulnerable(Estimulo, Objeto)), fail.
aprendizaje( evento(impacta(Estimulo, Objeto), perjudica(Estimulo, Objeto)) ):-
		vulnerable(Estimulo, Objeto), !.
aprendizaje( evento(impacta(Estimulo, Objeto), perjudica(Estimulo, Objeto)) ):-
		assertz(vulnerable(Estimulo, Objeto)),
		assertz(destruible(Objeto)), !.


% Que objetos llegan a explotar, es decir, pueden ser destruidos.
aprendizaje( evento(impacta(Estimulo, Objeto), explota(Objeto)) ):-
		not(vulnerable(_, Objeto)), assertz(vulnerable(Estimulo, Objeto)), fail.
aprendizaje( evento(impacta(Estimulo, Objeto), explota(Objeto)) ):-
		invulnerable(Estimulo, Objeto), retract(invulnerable(Estimulo, Objeto)),
		fail.
aprendizaje( evento(impacta(_, Objeto), explota(Objeto)) ):-
		destruible(Objeto), !.
aprendizaje( evento(impacta(_, Objeto), explota(Objeto)) ):-
		assertz(destruible(Objeto)), !.


% Que cosas benefician a un objeto.
aprendizaje( evento(impacta(PowerUp, Objeto), beneficia(PowerUp, Objeto)) ):-
		vulnerable(PowerUp, Objeto), retract(vulnerable(PowerUp, Objeto)), fail.
aprendizaje( evento(impacta(PowerUp, Objeto), beneficia(PowerUp, Objeto)) ):-
		not(vulnerable(_, Objeto)), retract(destruible(Objeto)), fail.
aprendizaje( evento(impacta(PowerUp, Objeto), beneficia(PowerUp, Objeto)) ):-
		mejora(PowerUp, Objeto), !.
aprendizaje( evento(impacta(PowerUp, Objeto), beneficia(PowerUp, Objeto)) ):-
		assertz(mejora(PowerUp, Objeto)), !.


% Que cosas no dañan a un objeto: disparos a los asteroides blancos ...
aprendizaje( evento(impacta(Estimulo, Objeto), evento_nulo) ):-
		vulnerable(Estimulo,Objeto), retract(vulnerable(Estimulo,Objeto)), fail.
aprendizaje( evento(impacta(_, Objeto), evento_nulo) ):-
		not(vulnerable(_,Objeto)), retract(destruible(Objeto)), fail.
aprendizaje( evento(impacta(Estimulo, Objeto), evento_nulo) ):-
		invulnerable(Estimulo, Objeto), !.
aprendizaje( evento(impacta(Estimulo, Objeto), evento_nulo) ):-
		assertz(invulnerable(Estimulo, Objeto)), !.


% La regla percepcion se encarga de recibir los "eventos" que suceden
% desde el codigo C# y aprende de lo que acontece alrededor de la nave de la IA.

percepcion([evento(Causa, Consecuencia)]):- 
	evento_valido(Causa), evento_valido(Consecuencia),
	aprendizaje(evento(Causa, Consecuencia)), !.
percepcion([evento(Causa, Consecuencia)|EventosRestantes]):-
	evento_valido(Causa), evento_valido(Consecuencia),
	aprendizaje(evento(Causa, Consecuencia)),
	percepcion(EventosRestantes).

% Eventos validos
% Causas
evento_valido(impacta(_,_)):- !.


% Consecuencias
evento_valido(evento_nulo):- !.
evento_valido(explota(_)):- !.
evento_valido(beneficia(_,_)):- !.
evento_valido(perjudica(_,_)):- !.
evento_valido( (EventoA, EventoB) ):-
		evento_valido(EventoA), evento_valido(EventoB).

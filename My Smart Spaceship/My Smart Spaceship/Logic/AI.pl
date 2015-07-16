% Reglas y hechos para manejar la logica de la inteligancia artificial

% Las reglas "peligroso" definen objetos que son dañinas para la ia o el humano.
% Las reglas "util" se refieren a powerups que benefician al usuario del objeto.
% Las reglas "arma" indican que un powerup sirve para dañar al humano en manos 
% del ia o viceversa.

:- dynamic peligroso_ia/1.
:- dynamic peligroso_humano/1.
:- dynamic util_ia/1.
:- dynamic util_humano/1.
:- dynamic arma_ia/1.
:- dynamic arma_humano/1.
:- dynamic destruible/1.
:- dynamic vulnerable/2.
:- dynamic invulnerable/2.

acercar(Objeto):- peligroso_ia(Objeto), !, fail.
acercar(Objeto):- not(util_ia(Objeto)), !, fail.
acercar(Objeto):- not(arma_ia(Objeto)), !, fail.
acercar(_):- !.

alejar(Objeto):- not(peligroso_ia(Objeto)), !, fail.
alejar(Objeto):- util_ia(Objeto), !, fail.
alejar(Objeto):- arma_ia(Objeto), !, fail.
alejar(_):- !.

disparar(Objeto):- indestructible(Objeto), !, fail.
disparar(Objeto):- util(Objeto), !, fail.
disparar(_):- !.

% La regla mover toma una lista de objetos que estan cerca de la IA (la cercania)
% se decide en el código en C#.
mover([], []):- !.
mover([], _).
mover([Objeto], [Objeto]). 
mover([Objeto|ListaObjetosCercanos], Acciones):- 
		alejar(Objeto), mover(ListaObjetosCercanos, L), Acciones = [alejar|L].
mover([Objeto|ListaObjetosCercanos], Acciones):- 
		acercar(Objeto), mover(ListaObjetosCercanos, L), Acciones = [acercar|L].

util(Objeto):- peligroso_ia(Objeto), !, fail.
util(Objeto):- arma_humano(Objeto), !, fail.
util(Objeto):- util_ia(Objeto), !.
util(Objeto):- arma_ia(Objeto), !.
util(_):- !.

% aprendizaje( evento(causa, consecuencia) ).

aprendizaje( evento(impacta(Estimulo, Objeto), perjudica(Estimulo, Objeto)) ):- 
		vulnerable(Estimulo, Objeto), !.
aprendizaje( evento(impacta(Estimulo, Objeto), perjudica(Estimulo, Objeto)) ):- 
		assertz(vulnerable(Estimulo, Objeto)), !.

aprendizaje( evento(impacta(Estimulo, Objeto), Consecuencia) ):- 
		not(Consecuencia == perjudica(Estimulo, Objeto)),
		invulnerable(Estimulo, Objeto), !.
aprendizaje( evento(impacta(Estimulo, Objeto), Consecuencia) ):- 
		not(Consecuencia == perjudica(Estimulo, Objeto)),
		assertz(invulnerable(Estimulo, Objeto)), !.

aprendizaje( evento(impacta(Estimulo, Objeto), explota(Objeto)) ):- 
		destruible(Objeto), !.
aprendizaje( evento(impacta(Estimulo, Objeto), explota(Objeto)) ):-
		assertz(destruible(Objeto)), !.

percepcion( evento(Causa, Consecuencia) ):- 
	evento_valido(Causa), evento_valido(Consecuencia),
	aprendizaje(evento(Causa, Consecuencia)), !.
percepcion((evento(Causa, Consecuencia),EventosRestantes)):- 
	evento_valido(Causa), evento_valido(Consecuencia),
	aprendizaje(evento(Causa, Consecuencia)), 
	percepcion(EventosRestantes).

% eventos validos
evento_valido(impacta(_,_)):- !.
evento_valido(explota(_)):- !.
evento_valido(mejora(_,_)):- !.
evento_valido(perjudica(_,_)):- !.
evento_valido( (EventoA, EventoB) ):- evento_valido(EventoA), evento_valido(EventoB).

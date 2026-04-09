**Imagine que HttpContext est une enveloppe. À l'intérieur, il y a deux feuilles : une feuille de Requête (ce que le client veut) et une feuille de Réponse (ce que tu lui renvoies).**

Voici les éléments clés que tu vas manipuler lundi :

1. context.Request (Ce qui entre)
C'est là que tu lis les intentions du client.

.Method : Savoir si c'est un GET (lecture), POST (création), PUT (modification) ou DELETE.

Importance : Indispensable pour la sécurité et le routage.

.Headers : C'est là que tu as cherché la clé X-Orange-Key. Les headers contiennent les métadonnées (le type de contenu, les jetons d'authentification).

.Path : L'URL appelée (ex: /test-sms).

.Body : C'est ici que se trouve le contenu JSON (ex: les détails du paiement).

.Connection.RemoteIpAddress : L'adresse IP de celui qui appelle. Crucial pour le bannissement ou la géolocalisation.

## context.Response (Ce qui sort)
C'est là que tu écris le résultat de ton travail.

.StatusCode : C'est le langage universel du Web.

200 : OK.

201 : Créé (après un paiement réussi).

400 : Bad Request (le client a fait une erreur).

401/403 : Problème de sécurité.

500 : Ton code a planté (ce qu'on gère dans ton middleware d'erreur).

.Headers : Tu peux y ajouter des infos, comme le temps de traitement ou des consignes de cache.

.WriteAsJsonAsync() : C'est l'outil qui transforme tes objets C# en texte JSON compréhensible par le téléphone du client.

## context.Items (Le sac à dos)
C'est une propriété moins connue mais très puissante pour les middlewares.

C'est quoi ? Un dictionnaire (Dictionary) qui vit uniquement le temps d'une seule requête.

Importance : Si ton SecurityMiddleware décode le nom de l'utilisateur, il peut le mettre dans context.Items["UserName"] = "Edgard".

Le bénéfice : Tous les middlewares suivants et tes routes pourront lire ce nom sans avoir à refaire le calcul. C'est comme une mémoire partagée temporaire.




3. ***context.Items (Le sac à dos)***
C'est une propriété moins connue mais très puissante pour les middlewares.

C'est quoi ? Un dictionnaire (Dictionary) qui vit uniquement le temps d'une seule requête.

Importance : Si ton SecurityMiddleware décode le nom de l'utilisateur, il peut le mettre dans context.Items["UserName"] = "Edgard".

Le bénéfice : Tous les middlewares suivants et tes routes pourront lire ce nom sans avoir à refaire le calcul. C'est comme une mémoire partagée temporaire.

4. **Pourquoi est-ce "Vital" pour toi** ?
Dans un microservice Orange Money :

Le *Middleware de Sécurité* regarde dans __context.Request.Headers__ pour vérifier le token.

Le *Middleware de Log regarde* dans __context.Request.Path__ pour savoir quelle API est appelée.

Le *Middleware d'Erreur* modifie __context.Response.StatusCode__ si ton service plante.

Ta *Route API* lit le __context.Request.Body__ pour savoir combien d'argent transférer.



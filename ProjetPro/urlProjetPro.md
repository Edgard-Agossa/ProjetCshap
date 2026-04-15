Méthode  : POST
URL      : http://localhost:5228/api/v1/clients
Headers  : Content-Type: application/json
Body (raw JSON) :
{
    "nom": "1XBET Bénin",
    "soldeMessages": 100
}

Méthode : GET
URL     : http://localhost:5228/api/v1/clients


3️⃣ Récupérer un Client par ID (GET)
Méthode : GET
URL     : http://localhost:5228/api/v1/clients/1

4️⃣ Recharger le solde (PUT)

Méthode : PUT
URL     : http://localhost:5228/api/v1/clients/1/recharger?montant=50


5️⃣ Envoyer un Message (POST)
Méthode  : POST
URL      : http://localhost:5228/api/v1/messages/envoyer/1
Headers  : Content-Type: application/json
Body (raw JSON) :
{
    "expediteur": "1XBET",
    "destinataire": "+22961000000",
    "contenu": "Votre code de vérification est : 4521",
    "type": 0
}


6️⃣ Récupérer les Messages d'un Client (GET)

Méthode : GET
URL     : http://localhost:5228/api/v1/messages/client/1

7️⃣ Mettre à jour le statut d'un Message (PATCH)
Méthode : PATCH
URL     : http://localhost:5228/api/v1/messages/1/statut?statut=Delivre

 Désactiver un Client (DELETE)
 Méthode : DELETE
URL     : http://localhost:5228/api/v1/clients/1



Tests de validation — ce qui doit échouer ✅
// Nom vide → doit retourner 400
POST /api/v1/clients
{ "nom": "", "soldeMessages": 100 }

// Numéro invalide → doit retourner 400
POST /api/v1/messages/envoyer/1
{ "expediteur": "TEST", "destinataire": "abc", "contenu": "test", "type": 0 }

// Client inexistant → doit retourner 404
GET /api/v1/clients/999

// Solde insuffisant → doit retourner 400
// (d'abord désactiver le client, puis essayer d'envoyer)
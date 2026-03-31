# TourGuide

Backend .NET pour le suivi de localisation d utilisateurs et la recommandation d attractions touristiques.

## Fonctionnalites

- Suivi GPS des utilisateurs en temps reel
- Recommandation des 5 attractions les plus proches
- Systeme de recompenses par visite
- Generation d offres de voyage personnalisees

## Prerequis

- .NET 7 SDK

## Installation

git clone https://github.com/AlexLeball/backend-dot-NET_P8.git
cd backend-dot-NET_P8
dotnet restore

## Lancement

dotnet run --project Api

Swagger UI disponible en mode Development : https://localhost:{port}/swagger

## Tests

dotnet test

Inclut des tests unitaires et des tests de performance (jusqu a 100 000 utilisateurs).

## CI/CD

Github Actions 
https://github.com/AlexLeball/backend-dot-NET_P8/blob/4fbf236d6ce7981d51f3fd160394e567ec0e0767/.github/workflows/ci.yml

## Branche

Branchez sur : develop

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Utilisateur (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nom TEXT NOT NULL,
    Identifiant TEXT NOT NULL UNIQUE,
    MotDePasseHash TEXT NOT NULL,
    Sel TEXT NOT NULL,
    ParametresHachage TEXT NOT NULL,
    Role TEXT NOT NULL CHECK (Role IN ('Administrateur', 'Vendeur')),
    Actif INTEGER NOT NULL DEFAULT 1 CHECK (Actif IN (0, 1))
);

CREATE TABLE IF NOT EXISTS Categorie (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nom TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Produit (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Reference TEXT NOT NULL UNIQUE,
    Nom TEXT NOT NULL,
    CategorieId INTEGER NULL,
    PrixVente NUMERIC NOT NULL CHECK (PrixVente > 0),
    SeuilAlerte INTEGER NOT NULL DEFAULT 0 CHECK (SeuilAlerte >= 0),
    Actif INTEGER NOT NULL DEFAULT 1 CHECK (Actif IN (0, 1)),
    DateCreation TEXT NOT NULL,
    FOREIGN KEY (CategorieId) REFERENCES Categorie(Id)
);

CREATE TABLE IF NOT EXISTS Vente (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Numero TEXT NOT NULL UNIQUE,
    JetonValidation TEXT NOT NULL UNIQUE,
    DateHeureUtc TEXT NOT NULL,
    DateMetier TEXT NOT NULL,
    VendeurId INTEGER NOT NULL,
    Total NUMERIC NOT NULL CHECK (Total > 0),
    ModePaiement TEXT NOT NULL CHECK (ModePaiement IN ('Especes', 'Mobile Money', 'Carte')),
    MontantRecu NUMERIC NOT NULL CHECK (MontantRecu >= 0),
    MonnaieRendue NUMERIC NOT NULL DEFAULT 0 CHECK (MonnaieRendue >= 0),
    Statut TEXT NOT NULL CHECK (Statut IN ('Validee', 'Annulee')),
    FOREIGN KEY (VendeurId) REFERENCES Utilisateur(Id)
);

CREATE TABLE IF NOT EXISTS LigneVente (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    VenteId INTEGER NOT NULL,
    ProduitId INTEGER NOT NULL,
    ReferenceSnapshot TEXT NOT NULL,
    NomSnapshot TEXT NOT NULL,
    Quantite INTEGER NOT NULL CHECK (Quantite > 0),
    PrixUnitaire NUMERIC NOT NULL CHECK (PrixUnitaire > 0),
    SousTotal NUMERIC NOT NULL CHECK (SousTotal > 0),
    UNIQUE (VenteId, ProduitId),
    FOREIGN KEY (VenteId) REFERENCES Vente(Id),
    FOREIGN KEY (ProduitId) REFERENCES Produit(Id)
);

CREATE TABLE IF NOT EXISTS AnnulationVente (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    VenteId INTEGER NOT NULL UNIQUE,
    AuteurId INTEGER NOT NULL,
    DateHeureUtc TEXT NOT NULL,
    Motif TEXT NOT NULL,
    FOREIGN KEY (VenteId) REFERENCES Vente(Id),
    FOREIGN KEY (AuteurId) REFERENCES Utilisateur(Id)
);

CREATE TABLE IF NOT EXISTS MouvementStock (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProduitId INTEGER NOT NULL,
    AuteurId INTEGER NOT NULL,
    DateHeureUtc TEXT NOT NULL,
    Type TEXT NOT NULL CHECK (
        Type IN ('StockInitial', 'Reapprovisionnement',
                 'Vente', 'AnnulationVente', 'CorrectionInventaire')
    ),
    QuantiteSignee INTEGER NOT NULL CHECK (QuantiteSignee <> 0),
    Motif TEXT NOT NULL,
    LigneVenteId INTEGER NULL,
    AnnulationId INTEGER NULL,
    FOREIGN KEY (ProduitId) REFERENCES Produit(Id),
    FOREIGN KEY (AuteurId) REFERENCES Utilisateur(Id),
    FOREIGN KEY (LigneVenteId) REFERENCES LigneVente(Id),
    FOREIGN KEY (AnnulationId) REFERENCES AnnulationVente(Id)
);

CREATE TABLE IF NOT EXISTS JournalAudit (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AuteurId INTEGER NULL,
    DateHeureUtc TEXT NOT NULL,
    Action TEXT NOT NULL,
    Entite TEXT NOT NULL,
    EntiteId INTEGER NULL,
    Details TEXT NOT NULL,
    FOREIGN KEY (AuteurId) REFERENCES Utilisateur(Id)
);

CREATE TABLE IF NOT EXISTS Parametre (
    Cle TEXT PRIMARY KEY,
    Valeur TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS VersionSchema (
    Numero INTEGER PRIMARY KEY,
    DateApplicationUtc TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS IX_Produit_Nom
ON Produit(Nom);

CREATE INDEX IF NOT EXISTS IX_Vente_Date
ON Vente(DateMetier);

CREATE INDEX IF NOT EXISTS IX_LigneVente_Produit
ON LigneVente(ProduitId);

CREATE INDEX IF NOT EXISTS IX_Mouvement_Produit_Date
ON MouvementStock(ProduitId, DateHeureUtc);

CREATE UNIQUE INDEX IF NOT EXISTS UX_Mouvement_Vente
ON MouvementStock(LigneVenteId)
WHERE Type = 'Vente';

CREATE UNIQUE INDEX IF NOT EXISTS UX_Mouvement_Annulation
ON MouvementStock(AnnulationId, LigneVenteId)
WHERE Type = 'AnnulationVente';

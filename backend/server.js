const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const handlers = require('./handlers/handler');
require('dotenv').config();

const app = express();

app.use(cors());
app.use(bodyParser.json({limit: '20mb'}));
app.use(bodyParser.urlencoded({limit: '20mb', extended: true}));

/*
    {
        documentOwner,
        documentPartner,
        hash,
        timestampOwner,
        signatureOwner,
        nicknameOwner,
        nicknamePartner,
        name
    }

    return ObjectID
 */
app.post('/upload', async (req, res) => await handlers.addDocument(req, res));

/*
    return {
        result: document{},
        error: null
    }
 */
app.get('/download/:id', async (req, res) => await handlers.getDocument(req,res));

/*
    {
        signaturePartner,
        timestampPartner

        return {
            signatureOwner,
            timestampOwner
        }
    }
 */
app.post('/sign/:id', async (req, res) => handlers.updateSignatures(req,res));

/*
    result: {
        documents
    }
 */
app.get('/documents/owner/:nickname', async (req, res) => handlers.getOwnerDocuments(req,res));

/*
    result: {
        documents
    }
 */
app.get('/documents/partner/:nickname', async (req, res) => handlers.getPartnerDocuments(req,res));

/*
    result: {
        proof,
        document
    }
 */
app.get('/proof/:txNumber/:hash', (req, res) => handlers.getProof(req, res));


app.listen(3000);
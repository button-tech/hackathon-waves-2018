const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const handlers = require('./handlers/handler');
require('dotenv').config();

const app = express();

app.use(cors());
app.use(bodyParser.json({limit: '20mb'}));
app.use(bodyParser.urlencoded({limit: '20mb', extended: true}));

app.get('/getFile/:hash', (req, res) => handlers.getDocument(req, res));
app.post('/data', async (req, res) => handlers.addDocument(req,res));
app.get('/proof/:txNumber/:hash/:timestamp', (req, res) => handlers.getProof(req, res));


app.listen(3000);
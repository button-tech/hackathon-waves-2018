const backendURL = "https://cd6c4867.ngrok.io";
const telegramServiceURL = "https://5ce5f1d2.ngrok.io/api/blockchain";

function getPrivateKey() {
    return document.getElementById("privateKey").value;
}

async function decryptAndDownload() {
    const { document, name } = await getDecryptedDocument();
    await download(name, document);
}

async function getDecryptedDocument() {
    const privateKey = getPrivateKey();
    const { publicKeyPartner, documentId } = await getPublicKeyAndDocumentID(getShortlink());
    // const keyPair = fromPvtKeyToKeyPair(privateKey);
    // if (getClientPublicKey(keyPair) != publicKeyPartner) {
    //     alert("Private Key неверный");
    //     return;
    // }
    const {
        dataPartner,
        name,
        hash
    } = await getDocument(documentId);
    return {
        document: decrypt(privateKey, dataPartner),
        name: name,
        hash: hash
    };
}

function getClientPublicKey(clientKeyPair) {
    return clientKeyPair.exportKey('pkcs1-public');
}

function fromPvtKeyToKeyPair(privateKey) {
    return new NodeRSA.RSA(privateKey);
}

async function sign() {
    const privateKey = getPrivateKey();
    const { documentId } = await getPublicKeyAndDocumentID(getShortlink());
    const keyPair = fromPvtKeyToKeyPair(privateKey);
    const {
        document1,
        name,
        hash,
    } = await getDecryptedDocument();
    const hashParent = CryptoJS.SHA256(document1).toString();
    const signaturePartner = signData(keyPair, hashParent);
    const timestampPartner = Date.now();
    // if (hashParent != hash) {
    //     alert("Хэши владельца документа и партнера не совпадают");
    //     return;
    // }
    const {
        signatureOwner,
        timestampOwner
    } = await sendSignature(documentId, signaturePartner, timestampPartner);

    document.getElementById("pes").innerHTML = `
    <h2>This is information that allow you to verify it</h2>
                    <h3>Please, save it</h3>
                    <div class="container">
                        <div class="row">
                            <table class="table table-bordered">
                                <tbody>
                                <tr>
                                    <td class="er" data-clipboard-text="0x${hashParent}"><strong>Hash:</strong>
                                      0x${hash}
                                    </td>
                                    <td class="er" data-clipboard-text="${new Date(timestampOwner).toDateString()}"><strong>
                                        Timestamp:</strong> ${new Date(timestampOwner).toDateString()}
                                    </td>
                                    <td class="er" data-clipboard-text="${signaturePartner}"><strong>My Signature
                                        :</strong> ${signaturePartner}
                                    </td>
                                    <td class="er" data-clipboard-text="${documentId}"><strong> Document
                                        Id:</strong> ${documentId}
                                    </td>
                                </tr>
                                </tbody>
                            </table>
                            <div class="text-center col-12">
                                <button class="btn btn-lg cont_button btn-info" onclick="$('.er').CopyToClipboard()"
                                        data-clipboard-target=".er">Copy
                                </button>
                            </div>
                        </div>
                    </div>`;

}

async function sendSignature(id, signature, timestamp) {
    return (await query("POST", `${backendURL}/sign/${id}`, JSON.stringify({
        signaturePartner: signature,
        timestampPartner: timestamp
    }))).result;
}

function signData(clientKey, data) {
    return clientKey.sign(data, 'hex');
}

function decrypt(privateKeyPartner, encryptedDocument) {
    const key = fromPvtKeyToKeyPair(privateKeyPartner);
    return key.decrypt(encryptedDocument);
}

function getPublicKeyAndDocumentID(guid) {
    return query("GET", `${telegramServiceURL}/documentId/${guid}`)
}

function download(filename, data) {
    var element = document.createElement('a');
    element.setAttribute('href', data);
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

async function getDocument(id) {
    return (await query("GET", `${backendURL}/download/${id}`)).result.document;
}

function getShortlink() {
    const demand = ['create'];
    const url = window.location;
    const urlData = parseURL(url);

    demand.forEach((property) => {
        if (urlData[property] === undefined)
            throw new Error('URL doesn\'t contain all properties');

    });

    return urlData.create;
}

function parseURL(url) {
    try {
        const params = url.search.substring(1);
        return JSON.parse(
            '{"' +
            decodeURI(params)
                .replace(/"/g, '\\"')
                .replace(/&/g, '","')
                .replace(/=/g, '":"') +
            '"}'
        );
    } catch (e) {
        addError("Pes not defined");
        throw e;
    }
}

/**
 * Request to server side
 * @param method Using method
 * @param url URL to send
 * @param data request data
 * @returns {Promise<*>}
 */
async function query(method, url, data) {
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": url,
        "method": method,
        "processData": false,
    };

    if (data) {
        settings.data = data;
        settings.headers = {
            "Content-Type": "application/json"
        };
    }

    const result = await $.ajax(settings);
    return result;
};

const backendURL = "http://localhost:3000";
const telegramServiceURL = "";

function getPrivateKey() {
    return document.getElementById("privateKey").value;
}

async function decryptAndDownload() {
    const { decryptedDocument, name } = await getDecryptedDocument();
    await download(name, decryptedDocument);
}

async function getDecryptedDocument() {
    const privateKey = getPrivateKey();
    const { publicKeyPartner, id } = getPublicKeyAndDocumentID();
    const keyPair = fromPvtKeyToKeyPair(privateKey);
    if (getClientPublicKey(keyPair) != publicKeyPartner) {
        alert("Private Key неверный");
        return;
    }
    const {
        encryptedDocumentPartner,
        name,
        hash
    } = await getDocument(id);
    return {
        document: decrypt(privateKey, encryptedDocumentPartner),
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
    const { id } = getPublicKeyAndDocumentID();
    const keyPair = fromPvtKeyToKeyPair(privateKey);
    const {
        encryptedDocumentPartner,
        hash,
    } = await getDecryptedDocument();
    const signaturePartner = signData(keyPair, encryptedDocumentPartner);
    const timestampPartner = Date.now();
    const hashParent = CryptoJS.SHA256(encryptedDocumentPartner);
    if (hashParent != hash) {
        alert("Хэши владельца документа и партнера не совпадают");
        return;
    }
    const {
        signatureOwner,
        timestampOwner
    } = await sendSignature(signaturePartner, timestampPartner);

    document.getElementById("show2").innerHTML = `
    <h2>This is information that allow you to verify it</h2>
                    <h3>Please, save it</h3>
                    <div class="container">
                        <div class="row">
                            <table class="table table-bordered">
                                <tbody>
                                <tr>
                                    <td class="er" data-clipboard-text="0x${hash}"><strong>Hash:</strong>
                                      0x${hash}
                                    </td>
                                    <td class="er" data-clipboard-text="${new Date(timestampOwner).toDateString()}"><strong>
                                        Timestamp:</strong> ${new Date(timestampOwner).toDateString()}
                                    </td>
                                    <td class="er" data-clipboard-text="${signatureOwner}"><strong>Owner Signature
                                        :</strong> ${signatureOwner}
                                    </td>
                                    <td class="er" data-clipboard-text="${signaturePartner}"><strong>My Signature
                                        :</strong> ${signaturePartner}
                                    </td>
                                    <td class="er" data-clipboard-text="${id}"><strong> Document
                                        Id:</strong> ${id}
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

function sendSignature(signature, timestamp) {
    return query("POST", `${backendURL}/sign/${id}`, JSON.stringify({
        signaturePartner: signature,
        timestampPartner: timestamp
    }));
}

function signData(clientKey, data) {
    return clientKey.sign(data);
}

function decrypt(privateKeyPartner, encryptedDocument) {
    const key = fromPvtKeyToKeyPair(privateKeyPartner);
    return key.encrypt(encryptedDocument, 'base64');
}

function getPublicKeyAndDocumentID() {

}

function download(filename, data) {
    const pom = document.createElement('a');
    pom.setAttribute('href', data);
    pom.setAttribute('download', filename);

    if (document.createEvent) {
        const event = document.createEvent('MouseEvents');
        event.initEvent('click', true, true);
        pom.dispatchEvent(event);
    }
    else {
        pom.click();
    }
}

function getDocument(id) {
    return query("GET", `${backendURL}/download/${id}`);
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

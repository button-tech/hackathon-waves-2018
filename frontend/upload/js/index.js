const backendURL = "http://localhost:3000";
const telegramServiceURL = "";

async function uploadFile() {
    const { data, name } = await getFile();
    const privateKey = document.getElementById('privateKey').value;
    const keyPair = fromPvtKeyToKeyPair(privateKey);

    const {
        publicKeyOwner,
        publicKeyPartner,
        nicknameOwner,
        nicknamePartner
    } = await getData();

    if (getClientPublicKey(keyPair) != publicKeyOwner) {
       alert("Private Key неверный");
       return;
    }

    const hash = getHash(data);
    const signature = signData(keyPair, hash);
    const encryptedDocumentOwner = encryptData(publicKeyOwner, data);
    const encryptedDocumentPartner = encryptData(publicKeyPartner, data);
    const timestampOwner = Date.now();

    const { result, error } = await sendUploadedFile(
        encryptedDocumentOwner,
        encryptedDocumentPartner,
        hash,
        timestampOwner,
        signature,
        nicknameOwner,
        nicknamePartner,
        name
        );

    if (error == null) {
        const id = result;

        document.getElementById("show2").innerHTML = `
         <h2>This is information that allow you to verify it</h2>
                    <h3>Please, save it</h3>
                    <div class="container">
                        <div class="row">
                            <table class="table table-bordered">
                                <tbody>
                                <tr>
                                    <td class="er" data-clipboard-text="0x${hash}"><strong>Hash:</strong>
                                        <span id="hash">0x${hash}</span>
                                    </td>
                                    <td class="er" data-clipboard-text="${new Date(timestampOwner).toDateString()}"><strong>
                                        Timestamp:</strong> <span id="timestamp">${new Date(timestampOwner).toDateString()}</span>
                                    </td>
                                    <td class="er" data-clipboard-text="${signature.toString()}"><strong> My Signature:</strong>
                                        <span id="signature">${signature.toString()}</span>
                                    </td>
                                    <td class="er" data-clipboard-text="${id}"><strong> Document
                                        Id:</strong> <span id="documentID">${id}</span>
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
    } else {
        alert("Ошибка при загрузке");
    }
}

function getClientPublicKey(clientKeyPair) {
    return clientKeyPair.exportKey('pkcs1-public');
}

async function sendUploadedFile(
    documentOwner,
    documentPartner,
    hash,
    timestampOwner,
    signatureOwner,
    nicknameOwner,
    nicknamePartner,
    name
) {

}

async function getData() {
    return {};
}

/**
 * Allows to encrypt data using RSA
 * @param serverPublicKey Public key of server side
 * @param data Data to encrypt
 * @returns {String} Encrypted data
 */
function encryptData(publicKey, data) {
    const key = new NodeRSA.RSA(publicKey, 'pkcs1-public');
    return key.encrypt(data, 'base64');
}

function fromPvtKeyToKeyPair(privateKey) {
    return new NodeRSA.RSA(privateKey);
}

/**
 * Allows to sign data using RSA
 * @param clientKey client Private key (key pair)
 * @param data Data to sign
 * @returns {Object} Sign data
 */
function signData(clientKey, data) {
    return clientKey.sign(data, 'hex');
}

function getHash(data) {
    return CryptoJS.SHA256(data);
}

/**
 * Get client file data
 * @returns {Promise<String>} file data (base64)
 */
function getFile() {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        const file = document.querySelector('input[type=file]').files[0];

        if (!file)
            throw new Error('You didn\'t add a file');
        reader.readAsDataURL(file);
        reader.onloadend = () => {
            resolve({data: reader.result, name: file.name});
        };
    });
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

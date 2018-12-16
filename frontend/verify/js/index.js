const backendURL = "http://localhost:3000";
const telegramServiceURL = "";
const masterAddress = "3N1yku2yYUB1QkQKsRR3fX5Dv9LxJFWk2gm";

async function GetDataFromIPFS(data){
        const rawResponse = await fetch('http://localhost:8080/get/'+data);
        return rawResponse.json();
}

async function GetRootHashFromBlockchain(address){
    let url = "https://testnodes.wavesnodes.com/addresses/data/" + address + "/rootHash"
    const response = await fetch(url);
    return response.json();
}

async function Verify(){
    const id = "5c1549c84be32e9d5decb985";
    const { index, hash, signatures } = (await query("GET", `${backendURL}/download/${id}`)).result.document;
    const concatSignatures = signatures.reduce((acc, val) => acc + val);
    const leaf = CryptoJS.SHA256(hash + concatSignatures).toString();
    let response = await fetch(`${backendURL}/proof/${index}/${hash}`)
    const proof = (await response.json()).result.proof;
    const { value } = await GetRootHashFromBlockchain(masterAddress);
    console.log(proof)
    console.log(verify(proof, leaf, value.substring(2), CryptoJS.SHA256))
}

async function getUserData() {

}

function getDocumentsByNickname(nickname) {
    return query("GET", `${backendURL}/documents/owner/${nickname}`);
}

function verifySignature(encryptedDocument, signature) {
    return key.verify(encryptedDocument, signature, 'string', 'hex')
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

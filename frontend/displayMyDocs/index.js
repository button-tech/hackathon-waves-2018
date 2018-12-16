const backendURL = "http://localhost:3000";
const ipfsServiceURL = 'http://localhost:8080/get/';

async function getDocumentsByNickname(nickname) {
    return (await query("GET", `${backendURL}/documents/owner/${nickname}`)).result.documents;
}

async function GetDataFromIPFS(commitHash){
    const rawResponse = await fetch(`${ipfsServiceURL}/${commitHash}`);
    return rawResponse.json();
}

(async () => {
    const nickname = "fsdfsdf";
    const documents = await getDocumentsByNickname(nickname);
    const privateKey = document.getElementById("privateKey").value;
    let counter = 1;
    for (d of documents) {
        const encryptedData = await GetDataFromIPFS(d.ipfsDataHashOwner);
        document.getElementById("tbl").innerHTML += `
                            <tr>
                        <th scope="row">${counter}</th>
                        <td>${d.nicknamePartner}</td>
                        <td>${d.signatures[1]}</td>
                        <td>0x${d.hash}</td>
                        <td><a href="#">Verify</a></td>
                        <td><a href="#">Download</a></td>
                    </tr>
        `;
        counter++;
    }
})();

function fromPvtKeyToKeyPair(privateKey) {
    return new NodeRSA.RSA(privateKey);
}


function decrypt(privateKeyPartner, encryptedDocument) {
    const key = fromPvtKeyToKeyPair(privateKeyPartner);
    return key.encrypt(encryptedDocument, 'base64');
}

function download(filename, text) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
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
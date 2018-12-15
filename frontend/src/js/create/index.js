checkPwd();

// TODO

// const linkToBot = `
//     <br>
//     <h5>
//         <a href="h" style="text-decoration: underline">
//            
//         </a>
//     </h5>
// `;


function startTimer(duration, display) {
    let timer = duration, minutes, seconds;
    const bomb = setInterval(function () {
        minutes = parseInt(timer / 60, 10)
        seconds = parseInt(timer % 60, 10);

        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;

        display.textContent = minutes + ":" + seconds;

        if (document.getElementById('loader').style.display == '')
            closeLoader();

        if (--timer < 0) {
            addWarning("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта", linkToBot);
            clearInterval(bomb)
        }
    }, 1000);
}

// TODO

(async () => {
    const deleteDate = await getLinkLivetime();
    const now = Date.now();
    const difference = Number(deleteDate) - now;
    if (difference <= 0) {
        addWarning("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта", linkToBot);
        throw new Error("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта");
    }

    const differenceInMinute = difference / 1000 / 60;
    const minutes = 60 * differenceInMinute,
        display = document.querySelector('#time');
    startTimer(minutes, display);
 
})();


async function getLinkLivetime() {
    const link = getShortlink();
    // try {
    //     const response = await req('GET', `${backendURL}/api/blockchain/validator/${link}`);
    //     if (response.error){
    //         addWarning("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта", linkToBot);
    //         return response.error;
    //     }
    //     else
    //         return new Date(response.result).getTime();
    // } catch (e) {
    //     addWarning("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта", linkToBot);
    //     throw new Error("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта");
    // }
    return 9999999999999;
}


async function generatePicture() {
    const password = checkPassword('password1', 'password2', 'error');
    if (!password) {
        return;
    }
    openLoader();

    const privateKeys = await getAllPrivateKeys();
    const addresses = await getAllAddresses(privateKeys);

    const encrypted = encryptAccount(privateKeys, password);

    const shortLink = getShortlink();

    // await sendAddresses(addresses, shortLink);

    document.getElementById('main').innerHTML = `
        <div class="container text-center">
            <br>
            <br>
            <h1>QR код - это доступ в Ваш аккаунт</h1>
            <br>
            <h5>Сохраните в надежное место и не потеряйте его!</h5>
            <br>
            <br>
            <img id="qr">
            <img id="qr1" style="display: none">
            <br>
            <br>
            <p id="save-qr-code"></p>
        </div>
        <div class="row">
            <div class="col-12 text-center">
                <br>
                <br>
                <br>
                <br>
                <br>
                <h1>Адрес Waves аккаунта</h1>
                <br>
                <br>
                <p style="font-size: 22px; word-wrap: break-word">Waves: ${addresses.Waves}</p>
            </div>
        </div>
    `;

    await createQRCode('qr', 'qr1', encrypted);
    addSaveButton('qr');
    closeLoader();
}

async function sendAddresses(addresses, shortlink) {
    const queryURL = `${backendURL}/api/blockchain/create/${shortlink}`;
    const data = {
        wavesAddress: addresses.Waves
    };
    try {
        const response = await req('PUT', queryURL, JSON.stringify(data));
        return response;
        if (response.error != null)
            throw new Error("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта");
    } catch (e) {
        addWarning("Вам будет выслана новая ссылка, если вы еще не имеете аккаунта", linkToBot);
        return e;
    }

}

function createQRCode(tagForQR, tagForSaveQR, data) {
    return new Promise((resolve, reject) => {
        (function () {
            const qr = new QRious({
                element: document.getElementById(tagForQR),
                value: data
            });
            qr.size = 300;
        })();
            resolve(true);
    });
}

function addSaveButton(tag) {
    const image = document.getElementById(tag);
    document.getElementById('save-qr-code').innerHTML = `<a href="${image.src}" download="WavesHack.png"><button class="btn btn-success">Download</button></a>`;
}

function encryptAccount(privateKeys, password) {
    if (password == '')
        throw new Error('Enter password');
    const encrypted = CryptoJS.AES.encrypt(JSON.stringify(privateKeys), password);
    return encrypted.toString();
}

function checkPassword(passwordElemID, repeatPasswordElemID, errorElemID) {
    const password = document.getElementById(passwordElemID).value;
    const repeatPassword = document.getElementById(repeatPasswordElemID).value;

    const checkObject = {
        0: {
            check: password != '',
            errorMessage:"Введите пароль"
        },
        1: {
            check: password.length >= 8,
            errorMessage: "Пароль меньше 8-ми символов"
        },
        2: {
            check: RegExp(/[0-9]/).test(password),
            errorMessage: "Пароль должен содержать цифры"
        },
        3: {
            check: RegExp(/(?=.*[a-z])(?=.*[A-Z])/).test(password),
            errorMessage:"Символы в пароле должны быть разного регистра"
        },
        4: {
            check: password === repeatPassword,
            errorMessage: "Пароли не совпадают"
        }
    };

    let err = false;
    for (let i in checkObject) {
        if (!checkObject[i].check) {
            err = true;
            $(`#${errorElemID}`).text(`${checkObject[i].errorMessage}`);
            break;
        } else {
            $(`#${errorElemID}`).text(``);
        }
    }

    return err == false ? password : false;
}

async function getAllAddresses(privateKeys) {
    const waves = Waves.init();
    const addresses = {};
    for (let currency in privateKeys)
        addresses[currency] = waves.account.getAddress(privateKeys[currency]);
    return addresses;
}

async function getAllPrivateKeys() {
    const wavesInstance = Waves.init()
    const wavesAccount = wavesInstance.account.create();
    return {
        Waves: wavesAccount.phrase
    }
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

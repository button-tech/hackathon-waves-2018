function CloseAlert(arg) {
    // TODO: use if instead of case
    switch (arg) {
        case 0:
            document.getElementById("Error_pop").style.display = 'none';
            break;
        case 1:
            document.getElementById("Success_pop").style.display = 'none';
            break;
        default:
            break;
    }
}

function closeLoader() {
    document.getElementById('loader').style.display = 'none';
}

function openLoader() {
    document.getElementById('loader').style.display = 'block';
}

function addError(errorText) {
    document.getElementById('container').innerHTML = ` 
    <div class="alert alert-danger col-12" id="Error_pop">
        <div class="row">
            <div class="col-10">
                <h2>Error</h2>
                <h5>${errorText}</h5>
            </div>
            <div class="col-2">
                <button type="button" class="close" onclick="CloseAlert(0)">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        </div>
    </div>`;
}

function addHint(errorText) {
    document.getElementById('error').innerHTML = ` 
    <div class="alert alert-danger col-12" id="Error_pop">
        <div class="row">
            <div class="col-10">
                <h2>Error</h2>
                <h5>${errorText}</h5>
            </div>
            <div class="col-2">
                <button type="button" class="close" onclick="CloseAlert(0)">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        </div>
    </div>`;
}

function addWarning(errorText, linkToBot) {
    document.getElementById('container').innerHTML = ` 
    <div class="alert alert-warning col-12" id="Error_pop">
        <div class="row">
            <div class="col-10">
                <h2>Warning</h2>
                <h5>${errorText}</h5>
                ${linkToBot}
            </div>
            <div class="col-2">
                <button type="button" class="close" onclick="CloseAlert(0)">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        </div>
    </div>`;
}

function addSuccess(successText, linkToBot) {
    const linkToBotSafe = linkToBot || "";
    document.getElementById('success').innerHTML =  `
        <div class="alert alert-success col-12" id="Success_pop">
        <div class="row">
            <div class="col-10">
                <h2>Success</h2>
                <h5 style="word-wrap: break-word">${successText}</h5>
                ${linkToBotSafe}
            </div>
            <div class="col-2">
                <button type="button" class="close" onclick="CloseAlert(1)">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        </div>
    </div>`;
}

/**
 * Allows to parse url string
 * @param url {Location} windows.location
 * @returns {Object}
 */
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

async function req(method, url, data) {
    const settings = {
        async: true,
        crossDomain: true,
        url: url,
        method: method,
        processData: false
    };

    if (data) {
        settings.data = data;
        settings.headers = {
            "Content-Type": "application/json"
            // "Cache-Control": "no-cache"
        };
    }

    return await $.ajax(settings);
}


const explorers = {
    "Waves": {
        "mainnet": 'http://wavesexplorer.com/tx/',
        "testnet": 'https://testnet.wavesexplorer.com/tx'
    },
};



// TODO: make function that will be called in window.load event handler
function checkPwd() {
    $("input[type=password]").keyup(function(){

        const makeRed = (elem) => {
            elem.removeClass("fa-check");
            elem.addClass("fa-remove");
            elem.css("color","#FF0004");
        };

        const makeGreen = (elem) => {
            elem.removeClass("fa-remove");
            elem.addClass("fa-check");
            elem.css("color","#00A41E");
        };

        const password = $("#password1");

        // Contains at least 8 chars check
        const eightCharMsg = $("#8char");
        password.val().length >= 8
            ? makeGreen(eightCharMsg)
            : makeRed(eightCharMsg);

        // Contains at least one upper case char check
        const upperCaseMsg = $("#ucase");
        // TODO: fix Regex should check for the capital letter on other language than english
        new RegExp("[A-Z]+").test(password.val())
            ? makeGreen(upperCaseMsg)
            : makeRed(upperCaseMsg);

        // Contains at least one lower case char
        const lowerCaseMsg = $("#lcase");
        // TODO: fix Regex should check for the small letter on other language than english
        new RegExp("[a-z]+").test(password.val())
            ? makeGreen(lowerCaseMsg)
            : makeGreen(lowerCaseMsg);

        // Contains at least one number
        const hasNumberMsg = $("#num");
        new RegExp("[0-9]+").test(password.val())
            ? makeGreen(hasNumberMsg)
            : makeRed(hasNumberMsg);


        // User should specify password twice in order to be sure on what he enter
        const passwordsMatch = $("#pwmatch");
        const repeatPassword = $("#password2");
        password.val() === repeatPassword.val()
            ? makeGreen(passwordsMatch)
            : makeRed(passwordsMatch);
    });
}
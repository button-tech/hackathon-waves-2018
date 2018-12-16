/**
 * Convert Number to BigNumber
 * @param x Some number
 * @returns {*} BigNumber
 */
const tbn = (x) => new BigNumber(x);

// For fix 
// TODO: Rewrite this module
// Remember about _utils. in litecoin and zcash modules
const tw =  (x) => BigNumber.isBigNumber(x) ? x.times(1e8).integerValue() : tbn(x).times(1e8).integerValue()
const fw =  (x) => BigNumber.isBigNumber(x) ? x.times(1e-8).toNumber() : tbn(x).times(1e-8).toNumber()
//

function clean(array, deleteValue) {
    for (var i = 0; i < array.length; i++) {
        if (array[i] == deleteValue) {
            array.splice(i, 1);
            i--;
        }
    }
    return array;
};

const isArray = (variable) => variable instanceof Array;
const toArray = (variable, length) => Array.from({length: length}, (v, k) => variable);
const toArrays = (...variables) => {
    const lengths = variables.map(elem => isArray(elem) ? elem.length : 1);
    const maxLength = lengths.reduce((acc, val) => val > acc ? val : acc, 0);
    const arrays = variables.map(elem => isArray(elem) ? elem : toArray(elem, maxLength));
    return {
        maxLength: maxLength,
        arrays: arrays
    };
};

const HexToUint8Array = (hexString) => new Uint8Array(hexString.match(/.{1,2}/g).map(byte => parseInt(byte, 16)));
const hasDuplicates = (array) => (new Set(array)).size !== array.length;
const isLengthError = (length, ...arrays) => arrays.reduce((acc, array) => acc === false && array.length === length ? false : true, false);
const isTxComplete = (utxoAmount, necessaryAmount) => utxoAmount >= necessaryAmount ? tbn(utxoAmount).minus(necessaryAmount).toNumber() : false;
const totalAmount = (amountArray) => amountArray.reduce((acc, val) => acc + val);
const isObject = (variable) => typeof variable == 'object';
const isNumber = (variable) => typeof variable == 'number';
const dynamicSort = (property) => {
    let sortOrder = 1;
    if(property[0] === "-") {
        sortOrder = -1;
        property = property.substr(1);
    }
    return function (a,b) {
        let result = (a[property] < b[property]) ? -1 : (a[property] > b[property]) ? 1 : 0;
        return result * sortOrder;
    }
};
const query = async (method, url, data) => {
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
            "Content-Type": "application/json",
            // "Cache-Control": "no-cache"
        };
    }

    const result = await $.ajax(settings);
    return result;
};


const _utils = {
    
    init: function () {

        return {

            course: {
                /**
                 * Allows to get course
                 * @param currency Currency, relative to which the course will be taken
                 * @returns {Promise<*>} Object with exchange rates
                 */
                getCourse: async (currency) => {
                    const response = await $.get({
                        url: `https://min-api.cryptocompare.com/data/price?fsym=${currency}&tsyms=WAVES,BTC,ETH,ZEC,LTC,USD,EUR,RUB`,
                        type: 'GET',
                    });
                    return response;
                },
                /**
                 * Allows to convert currencies
                 * @param from Currency that will be changed
                 * @param to Destination currency
                 * @param value Amount of currency that will be changed
                 * @returns {Promise<number>}
                 */
                convert: async (from, to, value) => {
                    const courses = await _utils.course.getCourse(currency[from].ticker);
                    const rate = courses[currency[to].ticker];
                    const result = value * rate;
                    return result * 1000 % 10 === 0 ? result : result.toFixed(2);
                }
            },
            /**
             * Convert Number (or BN) to this * 10^8
             * @param x Some number
             * @returns {*} Number * 10^8
             */
            tw: (x) => BigNumber.isBigNumber(x) ? x.times(1e8).integerValue() : tbn(x).times(1e8).integerValue(),

            /**
             * Convert from minimal Waves unit
             * @param x Some number
             * @returns {*} Converted number
             */
            fw: (x) => BigNumber.isBigNumber(x) ? x.times(1e-8).toNumber() : tbn(x).times(1e-8).toNumber()
        }
    }
};

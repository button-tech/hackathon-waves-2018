const Waves = (function () {

    let network = {};

    return {

        init: function () {


            const Waves = WavesAPI.create(WavesAPI.MAINNET_CONFIG);


            function getSeedFromPhrase(phrase) {
                const seed = Waves.Seed.fromExistingPhrase(phrase);
                return seed;
            }

            async function assets(address) {
                const user = await Waves.API.Node.assets.balances(address);
                return user.balances;
            }

            async function getOrderBook(currencyToSell, currencyToBuy) {
                const sell = currency[currencyToSell].assetID;
                const buy = currency[currencyToBuy].assetID;

                const response = await $.get({
                    url: `https://matcher.wavesplatform.com/matcher/orderbook/${sell}/${buy}`,
                    type: 'GET',
                    dataType: 'text'
                });

                return JSON.parse(response);
            }

            async function getOrderProperties(currencyToSell, currencyToBuy, amount) {
                const orderBook = await getOrderBook(currencyToSell, currencyToBuy);

                const type = exchange[currencyToSell][currencyToBuy].type;

                let dependency;
                if (type === 'sell') {
                    dependency = 'bids';
                } else {
                    dependency = 'asks';
                }

                let priceIndex;
                for (let i in orderBook[dependency]) {
                    const price = orderBook[dependency][i].price;
                    let _amount = amount;
                    if (type === 'sell')
                        _amount = tw(tbn(amount).div(price)).toNumber();
                    if (orderBook[dependency][i].amount >= _amount) {
                        priceIndex = i;
                        amount = _amount;
                        break;
                    }
                }

                const price = orderBook[dependency][priceIndex].price;

                return {
                    amountAsset: orderBook.pair.amountAsset,
                    priceAsset: orderBook.pair.priceAsset,
                    type: type,
                    amount: amount,
                    price: price,
                }
            }

            async function createTXObject(amountAssetId, priceAssetId, orderType, amount, price, expiration, sender) {
                const matcherPublicKey = await Waves.API.Matcher.getMatcherKey();

                const transferData = {
                    senderPublicKey: sender.keyPair.publicKey,
                    matcherPublicKey: matcherPublicKey,
                    amountAsset: amountAssetId,
                    priceAsset: priceAssetId,
                    orderType: orderType,
                    amount: amount,
                    price: price,
                    timestamp: Number(Date.now()),
                    expiration: Number(Date.now() + expiration),
                    matcherFee: 300000
                };

                return transferData;
            }

            return {

                network: network,
                account: {
                    /**
                     * Allows to create new key pairs
                     * @returns {Seed} Seed Object (phrase, address, keyPair)
                     */
                    create: () => {
                        const seed = Waves.Seed.create();
                        return seed;
                    },
                    /**
                     * Allows to encrypt seed phrase
                     * @param seed User's seed phrase
                     * @param password Encryption key
                     * @returns {*} Encrypted Seed
                     */
                    encrypt: (phrase, password) => {
                        const seed = getSeedFromPhrase(phrase);
                        const encrypted = seed.encrypt(password);
                        return encrypted;
                    },
                    /**
                     * Allows to decrypt seed phrase
                     * @param encryptedSeed Encrypted seed
                     * @param password Decryption key
                     * @returns {string} Seed Phrase
                     */
                    decrypt: (encryptedSeed, password) => {
                        const restoredPhrase = Waves.Seed.decryptSeedPhrase(encryptedSeed, password);
                        return restoredPhrase;
                    },
                    /**
                     * Get seed object from seed phrase
                     * @param phrase A set of 15 words
                     * @returns {Seed} {Seed} Seed Object (phrase, address, keyPair)
                     */
                    getSeedFromPhrase: getSeedFromPhrase,
                    /**
                     * Allows to get address from seed phrase
                     * @param phrase A set of 15 words
                     * @returns {*} User's address
                     */
                    getAddress: (phrase) => {
                        return getSeedFromPhrase(phrase).address;
                    },
                },
                balance: {
                    /**
                     * Allows to get Waves balance
                     * @param address User's address
                     * @returns {Promise<any>} balance
                     */
                    waves: async (address) => {
                        const balance = await Waves.API.Node.addresses.balance(address, 6);
                        return balance;
                    },
                    /**
                     * Allows to get balance of all assets excluding Waves
                     * @param address User's address
                     * @returns {Promise<balances|((address: string) => Promise<any>)|((address: string) => Promise<any>)>} Array of assets balances
                     */
                    assets: assets,
                    /**
                     * Allows to get balance of any one asset
                     * @param address User's address
                     * @param assetID ID of asset
                     * @returns {Promise<*>} balance
                     */
                    byAssetID:
                        async (address, assetID) => {
                            const user = await Waves.API.Node.assets.balance(address, assetID);
                            return user.balance;
                        },
                    /**
                     * Allows to get object with balances of any assetsId
                     * @param address User's address
                     * @param assetsID Array or Object includes assetsId of currencies
                     * @returns {Promise<void>} Object of balances
                     */
                    byAssetIDArray:
                        async (address, assetsID) => {
                            const assetsBalance = await assets(address);

                            let balances = {};

                            for (let i in assetsID) {
                                assetsBalance.map(asset => {
                                    if (asset.assetId == assetsID[i])
                                        balances[i] = asset.balance
                                });
                            }

                            return balances;
                        },
                },
                transactions: {
                    /**
                     * Allows to sign transaction
                     * @param seedPhrase A set of 15 words
                     * @param to User that will get currency
                     * @param assetId AssetId of the currency
                     * @param amount Amount of currency
                     * @returns {Promise<any>} Signed object of transaction
                     */
                    signTransaction: async (seedPhrase, to, assetId, amount) => {
                        const sender = getSeedFromPhrase(seedPhrase);

                        const transferData = {
                            type: Waves.constants.TRANSFER_TX,
                            sender: sender.address,
                            recipient: to,
                            senderPublicKey: sender.keyPair.publicKey,
                            assetId: assetId,
                            amount: amount,
                            feeassetID: assets.Waves,
                            fee: 100000,
                            attachment: '',
                            timestamp: Date.now()
                        };

                        let signedTransaction = await Waves.API.Node.transactions.getSignature('transfer', transferData, sender.keyPair);

                        let data = JSON.parse(signedTransaction.body);
                        data.type = Waves.constants.TRANSFER_TX;

                        return data;
                    },
                    /**
                     * Allows to send signed transaction
                     * @param data Signed transaction object
                     * @returns {Promise<void>}
                     */
                    sendSigned: async (data) => {
                        const responseData = await Waves.API.Node.transactions.rawBroadcast(data);
                        return responseData;
                    },
                    /**
                     * Allows to sign and send a transaction (currency)
                     * @param recipient User that will get currency
                     * @param assetId AssetId of the currency
                     * @param amount Amount of currency
                     * @param seedPhrase A set of 15 words
                     * @returns {Promise<any>} Signed object of transaction
                     */
                    send: async (recipient, assetId, amount, seedPhrase) => {
                        const sender = getSeedFromPhrase(seedPhrase);

                        const transferData = {
                            type: Waves.constants.TRANSFER_TX,
                            sender: sender.keyPair.address,
                            recipient: recipient,
                            senderPublicKey: sender.keyPair.publicKey,
                            assetId: assetId,
                            amount: amount,
                            feeassetID: assets.Waves,
                            fee: 100000,
                            attachment: '',
                            timestamp: Date.now()
                        };

                        const responseData = await Waves.API.Node.transactions.broadcast('transaction', transferData, sender.keyPair);
                        return responseData;
                    }
                },
                exchange: {
                    /**
                     * Allows to get asset pair with bids and asks
                     * @param currencyToSell Currency that will be sell
                     * @param currencyToBuy Currency that will be buy
                     * @returns {Promise<any>} Order book object
                     */
                    getOrderBook: getOrderBook,
                    /**
                     * Calculate order properties
                     * @param currencyToSell Currency that will be sell
                     * @param currencyToBuy Currency that will be buy
                     * @param amount Amount of currency that will be buy
                     * @returns {Promise<{amountAsset: *, priceAsset: *, type: *, amount: *, price: (*|content.price|{type, required})}>} Object with properties
                     */
                    getOrderProperties: getOrderProperties,
                    /**
                     * Allows to create tx object for exchange cryptocurrencies
                     * @param amountAssetId ID of asset that will be in amount field
                     * @param priceAssetId ID of asset that will be in price field
                     * @param orderType Type of order (buy or sell)
                     * @param amount Amount of currency that will be sell or buy
                     * @param price Exchange rate
                     * @param expiration After that time created and not used order will be closed
                     * @param sender Seed Object
                     * @returns {Promise<{senderPublicKey: *, matcherPublicKey, amountAsset: *, priceAsset: *, orderType: *, amount: *, price: *, timestamp: number, expiration: number, matcherFee: number}>} tx object
                     */
                    createTXObject: createTXObject,
                    /**
                     * Allows to sign exchange transaction
                     * @param currencyToSell Currency that will be sell
                     * @param currencyToBuy Currency that will be buy
                     * @param amount Amount of currency that will be buy
                     * @param expiration After that time created and not used order will be closed
                     * @param seedPhrase A set of 15 words
                     * @returns {Promise<any>} Sign transaction object
                     */
                    signTransaction: async (currencyToSell, currencyToBuy, amount, expiration, seedPhrase) => {
                        const sender = getSeedFromPhrase(seedPhrase);

                        const orderProperties = await getOrderProperties(currencyToSell, currencyToBuy, amount);

                        try {
                            const TX_OBJECT = await createTXObject(
                                orderProperties.amountAsset,
                                orderProperties.priceAsset,
                                orderProperties.type,
                                orderProperties.amount,
                                orderProperties.price,
                                expiration,
                                sender
                            );

                            let signedTransaction = await Waves.API.Matcher.signTransaction(TX_OBJECT, sender.keyPair);

                            let data = JSON.parse(signedTransaction.body);

                            return data;
                        } catch (e) {
                            throw e;
                        }
                    },
                    /**
                     * Allows to send signed exchange transaction
                     * @param data Signed transaction object
                     * @returns {Promise<void>}
                     */
                    sendSigned: async (data) => {
                        const transferData = {
                            method: 'POST',
                            headers: {
                                'Accept': 'application/json',
                                'Content-Type': 'application/json;charset=UTF-8'
                            },
                            body: JSON.stringify(data)
                        };

                        const responseData = await Waves.API.Matcher.send(transferData);
                        return responseData;
                    },
                    /**
                     * Allows to create order with min price in buy and max price in sell
                     * @param currencyToSell Currency that will be sell
                     * @param currencyToBuy Currency that will be buy
                     * @param amount Amount of currency that will be buy
                     * @param expiration After that time created and not used order will be closed
                     * @param seedPhrase A set of 15 words
                     * @returns {Promise<void>} Accepted order (or not accepted) object
                     */
                    createCustomOrder: async (currencyToSell, currencyToBuy, amount, expiration, seedPhrase) => {
                        const sender = getSeedFromPhrase(seedPhrase);

                        const orderProperties = await getOrderProperties(currencyToSell, currencyToBuy, amount);

                        try {
                            const TX_OBJECT = await createTXObject(
                                orderProperties.amountAsset,
                                orderProperties.priceAsset,
                                orderProperties.type,
                                orderProperties.amount,
                                orderProperties.price,
                                expiration,
                                sender
                            );

                            return await Waves.API.Matcher.createOrder(TX_OBJECT, sender.keyPair);
                        } catch (e) {
                            throw e;
                        }
                    },
                }

            }

        }

    }
}())
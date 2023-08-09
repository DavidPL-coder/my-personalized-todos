import fs from "fs";

export default class HttpsSecurityOptionsProvider {
    static getOptions(projectpath) {
        let options = {
            key: fs.readFileSync(projectpath + `/https/${process.env.MPT_KEY_FILE_NAME}`),
            cert: fs.readFileSync(projectpath + `/https/${process.env.MPT_CERT_FILE_NAME}`)
        }
        
        if (process.env.MPT_SSL_PASSWORD != undefined)
            options.passphrase = process.env.MPT_SSL_PASSWORD;

        return options;
    }
}


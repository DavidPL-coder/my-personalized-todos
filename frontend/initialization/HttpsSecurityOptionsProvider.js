import fs from "fs";

export default class HttpsSecurityOptionsProvider {
    static getOptions(projectpath) {
        const key = fs.readFileSync(projectpath + "/https/key.pem");
        const cert = fs.readFileSync(projectpath + "/https/cert.pem");

        return { key, cert, passphrase: 'qwerty' };
    }
}


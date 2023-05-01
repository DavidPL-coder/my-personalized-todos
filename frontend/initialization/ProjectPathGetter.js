export default class ProjectPathGetter {
    static getPath(filepath, nodeEnv) {
        let projectpath = filepath;

        if (nodeEnv === "development") { // TODO: Use env variable which has information about using docker
            const lastSlashIndex = filepath.lastIndexOf("/");
            const lastBackslashIndex = filepath.lastIndexOf("\\");
            const index = lastSlashIndex > 0 ? lastSlashIndex : lastBackslashIndex;
            projectpath = filepath.slice(0, index);
        }

        return projectpath;
    }
}
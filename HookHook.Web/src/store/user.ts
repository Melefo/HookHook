const user = {
    namespaced: true,
    state: {

    },
    mutations: {

    },
    actions: {
        async register(_: any, json: any) {
            const res = await fetch("/api/user/register", {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(json)
            })
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { error, errors } = await res.json();
                return { error, errors };
            }
            return {};
        },
        async login(_: any, json: any) {
            const res = await fetch("/api/user/login", {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(json)
            })
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { error, errors } = await res.json();
                return { error, errors };
            }
            return {};
        },
        async registerWithGithub(_: any, json: any)
        {
            const res = await fetch(`https://github.com/login/oauth/authorize?client_id=${json.clientID}&response_type=code&redirect_uri=${json.redirectURI}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "Accept": "application/json",
                    "Access-Control-Allow-Origin": "*",
                    "Access-Control-Allow-Mehotds": "GET, POST, PATCH, PUT, DELETE, OPTIONS",
                    "Access-Control-Allow-Headers": "Origin, Content-Type, X-Auth-Token"
                }
            })
            .then((response) => {
                console.log("Got response = ", response);
            })
            .catch((err) => {
                return ({error: err})
            })
            return res;
        }
    },
    getters: {

    }
}

export default user
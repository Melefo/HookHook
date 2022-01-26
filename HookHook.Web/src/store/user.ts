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
            if (res.status === 500)
                return { error: "Backend unavailable" };
            const contentType = res.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                const { error, errors } = await res.json();
                return { error, errors };
            }
            return {};
        }
    },
    getters: {

    }
}

export default user
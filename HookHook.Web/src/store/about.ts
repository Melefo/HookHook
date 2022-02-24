const about = {
    namespaced: true,
    state: {
        info: null
    },
    mutations: {
        get(state: any, info: any) {
            state.info = info;
        }
    },
    actions: {
        async get({ commit, state }: any) {
            if (state.info !== null) {
                return {};
            }
            const res = await fetch("/api/about.json", {
                method: 'GET'
            })
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                const json = await res.json();

                commit('get', json);
            }
            else if (contentType && contentType.indexOf("application/problem+json") !== 1) {
                const { error, errors } = await res.json();
                return { error, errors };
            }
            return {};
        }
    },
}

export default about
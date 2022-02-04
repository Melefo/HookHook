import { parseJwt } from "@/router";

const user = {
    namespaced: true,
    state: {
        token: null
    },
    mutations: {
        login(state: any, token: string) {
            state.token = token
        }
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
        async login({ commit }: any, json: any) {
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
                const { token, error, errors } = await res.json();
                commit('login', token);
                return { error, errors };
            }
            return {};
        },
        async discord({ commit }: any, code: String) {
            const res = await fetch("/api/user/oauth/discord?code=" + code, {
                method: 'POST',
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { token, error, errors } = await res.json();
                if (token) {
                    commit('login', token);
                }
                return { error, errors };
            }
            return {};
        },
        async github({ commit }: any, code: String) {
            const res = await fetch("/api/user/oauth/github?code=" + code, {
                method: 'POST',
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { token, error, errors } = await res.json();
                if (token) {
                    commit('login', token);
                }
                return { error, errors };
            }
            return {};
        },
        async spotify({ commit }: any, code: String) {
            const res = await fetch("/api/user/oauth/spotify?code=" + code, {
                method: 'POST',
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { token, error, errors } = await res.json();
                if (token) {
                    commit('login', token);
                }
                return { error, errors };
            }
            return {};
        },
        async twitch({ commit }: any, code: String) {
            const res = await fetch("/api/user/oauth/twitch?code=" + code, {
                method: 'POST',
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { token, error, errors } = await res.json();
                if (token) {
                    commit('login', token);
                }
                return { error, errors };
            }
            return {};
        },
        async twitter({ commit }: any, code: String) {
            const res = await fetch("/api/user/oauth/twitter?code=" + code, {
                method: 'POST',
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { token, error, errors } = await res.json();
                if (token) {
                    commit('login', token);
                }
                return { error, errors };
            }
            return {};
        }
    },
    getters: {
        isLoggedIn(state: any): Boolean {
            return !!state.token;
        },
        isAdmin(state: any): Boolean {
            return !!state.token && parseJwt(state.token).Role === "Admin";
        },
        token(state: any): String {
            return state.token;
        }
    }
}

export default user
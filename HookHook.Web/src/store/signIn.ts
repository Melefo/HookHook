import { parseJwt } from "@/router";
import { authHeader } from "@/store";

const signIn = {
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
            const res = await fetch("/api/signin/register", {
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
            const res = await fetch("/api/signin/login", {
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
        async logout({ commit }: any) {
            commit('login', null);
        },
        async discord({ commit }: any, code: String) {
            const res = await fetch("/api/signin/oauth/discord?code=" + code, {
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
            const res = await fetch("/api/signin/oauth/github?code=" + code, {
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
            const res = await fetch("/api/signin/oauth/spotify?code=" + code, {
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
            const res = await fetch("/api/signin/oauth/twitch?code=" + code, {
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
        async twitter({ commit }: any, { token, verifier }: any) {
            const res = await fetch(`/api/signin/oauth/twitter?code=${token}&verifier=${verifier}`, {
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
        async google({ commit }: any, code: String) {
            const res = await fetch("/api/signin/oauth/google?code=" + code, {
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
        async authorize({ commit }: any, provider: String) {
            const res = await fetch(`/api/signin/authorize/${provider}`, {
                method: 'GET',
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("text/plain") !== -1)) {
                const url = await res.text();
                return { url: url };
            }
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const { error, errors } = await res.json();
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
            const jwt = parseJwt(state.token);
            return !!jwt && jwt.role === "Admin";
        },
        token(state: any): String {
            return state.token;
        }
    }
}

export default signIn
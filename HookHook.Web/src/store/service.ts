import { authHeader } from ".";

const service = {
    namespaced: true,
    state: {
        accounts: {}
    },
    mutations: {
        getAccounts(state: any, {provider, info }: any) {
            state.accounts[provider] = info;
        },
        deleteAccount(state: any, { provider, key }: any) {
            state.accounts[provider].splice(key, 1);
        },
        addAccount(state: any, {provider, info}: any) {
            state.accounts[provider].push(info);
        }
    },
    actions: {
        async getAccounts({ commit, state }: any, provider: any) {
            const res = await fetch('/api/service/' + provider, {
                method: 'GET',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined)
                    return json;
                commit('getAccounts', { provider, info: json });
            }
            return {};
        },
        async deleteAccount({ commit }: any, { provider, id, key }: any) {
            const res = await fetch('/api/service/' + provider + '?id=' + id, {
                method: 'DELETE',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                return await res.json();
            }
            commit('deleteAccount', { provider, key });
            return {};
        },
        async addDiscord({ commit }: any, code: String) {
            const res = await fetch("/api/service/discord?code=" + code, {
                method: 'POST',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined)
                    return json;
                if (json.userId === undefined || json.username === undefined) {
                        return {};
                }
                commit('addAccount', { provider: 'Discord', info: json });
            }
            return {};
        },
        async addTwitter({ commit }: any, { token, verifier }: any) {
            const res = await fetch(`/api/service/twitter?code=${token}&verifier=${verifier}`, {
                method: 'POST',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined)
                    return json;
                if (json.userId === undefined || json.username === undefined) {
                        return {};
                }
                commit('addAccount', { provider: 'Twitter', info: json });
            }
            return {};
        },
        async addTwitch({ commit }: any, code: String) {
            const res = await fetch("/api/service/twitch?code=" + code, {
                method: 'POST',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined)
                    return json;
                if (json.userId === undefined || json.username === undefined) {
                        return {};
                }
                commit('addAccount', { provider: 'Twitch', info: json });
            }
            return {};
        },
        async addSpotify({ commit }: any, code: String) {
            const res = await fetch("/api/service/spotify?code=" + code, {
                method: 'POST',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined)
                    return json;
                if (json.userId === undefined || json.username === undefined) {
                        return {};
                }
                commit('addAccount', { provider: 'Spotify', info: json });
            }
            return {};
        },
        async addGitHub({ commit }: any, code: String) {
            const res = await fetch("/api/service/github?code=" + code, {
                method: 'POST',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined)
                    return json;
                if (json.userId === undefined || json.username === undefined) {
                        return {};
                }
                commit('addAccount', { provider: 'GitHub', info: json });
            }
            return {};
        },
        async addGoogle({ commit }: any, code: String) {
            const res = await fetch("/api/service/google?code=" + code, {
                method: 'POST',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined)
                    return json;
                if (json.userId === undefined || json.username === undefined) {
                        return {};
                }
                commit('addAccount', { provider: 'Google', info: json });
            }
            return {};
        },
    }
}

export default service
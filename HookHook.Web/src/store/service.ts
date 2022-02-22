import { authHeader } from ".";

const service = {
    namespaced: true,
    actions: {
        async getAccounts({ commit }: any, provider: String) {
            const res = await fetch('/api/service/' + provider, {
                method: 'GET',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                return await res.json();
            }
            return {};
        },
        async deleteAccount({ commit }: any, { provider, id }: any) {
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
                return await res.json();
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
                return await res.json();
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
                return await res.json();
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
                return await res.json();
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
                return await res.json();
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
                return await res.json();
            }
            return {};
        },
    }
}

export default service
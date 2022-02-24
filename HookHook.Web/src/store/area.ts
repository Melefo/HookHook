import { authHeader } from ".";

const area = {
    namespaced: true,
    state: {
        services: [] as any[],
        areas: [] as any[]
    },
    mutations: {
        getServices(state: any, services: any) {
            state.services = services;
        },
        getAreas(state: any, areas: any) {
            state.areas = areas;
        }
    },
    actions: {
        async getServices({ commit, state }: any) {
            if (state.services.length > 0) {
                return;
            }
            const res = await fetch("/api/area/getservices", {
                method: 'GET',
                headers: authHeader()
            })
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                const json = await res.json();
                commit('getServices', json);
            }
            else if (contentType && contentType.indexOf("application/problem+json") !== 1) {
                const { error, errors } = await res.json();
                return { error, errors };
            }
            return {};
        },
        async createAreaRequest(_: any, json: any) {
            const res = await fetch("/api/area/create", {
                method: 'POST',
                headers: {
                    ...authHeader(),
                    ... {
                        "Content-Type": "application/json"
                    }
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
        async get({ state, commit }: any) {
            const res = await fetch("/api/area/all", {
                method: 'GET',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && (contentType.indexOf("application/json") !== -1 || contentType.indexOf("application/problem+json") !== -1)) {
                const json = await res.json();
                if (json.error !== undefined || json.errors !== undefined) {
                    return json;
                }
                commit('getAreas', json);
            }
            return {};
        },
        async delete(_: any, id: string) {
            const res = await fetch("/api/area/delete/" + id, {
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
        }
    }
}

export default area
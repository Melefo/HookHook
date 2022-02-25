import { authHeader } from "@/store";

const user = {
    namespaced: true,
    state: {
    },
    mutations: {
    },
    actions: {
        async all({ commit }: any) {
            const res = await fetch("/api/user/all", {
                method: 'GET',
                headers: authHeader()
            });
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                const json = await res.json();
                return json;
            }
            if (contentType && contentType.indexOf("application/problem+json") !== -1) {
                const json = await res.json();
                return { error: json.error, errors: json.errors };
            }
            return {};
        },
        del({ commit }: any, id: string) {
            fetch("/api/user/delete/" + id, {
                method: "DELETE",
                headers: authHeader()
            })
        },
        promote({ commit }: any, id: string) {
            fetch("/api/user/promote/" + id, {
                method: "PATCH",
                headers: authHeader()
            })
        },
        trigger(_: any, id: string) {
            fetch("/api/user/trigger/" + id, {
                method: "GET",
                headers: authHeader()
            })
        }
    },
    getters: {
    }
}

export default user
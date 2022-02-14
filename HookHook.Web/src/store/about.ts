const about = {
    namespaced: true,
    actions: {
        async get(_: any) {
            const res = await fetch("/api/about.json", {
                method: 'GET'
            })
            if (res.status === 500) {
                return { error: "Backend unavailable" };
            }
            const contentType = res.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                const json = await res.json();

                return json;
            }
            if (contentType && contentType.indexOf("applicatoin/problem+json") !== 1) {
                const { error, errors } = await res.json();
                return { error, errors };
            }
            return {};
        }
    }
}

export default about
import cookies from 'cookie';

/** @type {import('./$types').PageServerLoad} */





/** @type {import('./$types').Actions} */
export const actions = {
    // @ts-ignore
    register: async ({ request }) => {

        const data = await request.formData();

        const username = data.get('username');
        const password = data.get('password');
        const email = data.get('email');

        const user = await authenticateUser({ username, password, email });

        async function authenticateUser(userCredentials: any) {
            let username = userCredentials.username;
            let password = userCredentials.password;
            let email = userCredentials.email;
            const response = await fetch('https://localhost:5000', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password, email })
            });

            if (response.ok) {

                return { success: true };

            } else {

                return { success: false };
            }
        }

    }
};


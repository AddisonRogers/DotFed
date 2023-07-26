<script>
    export let
        /** @type {import('./$types').PageData} */
        data,
        /** @type {import('./$types').ActionData} */
        form;
</script>

<div class="h-screen flex justify-center items-center bg-gray-900 dark">
    <div class="p-6 max-w-sm w-full bg-gray-800 shadow-md rounded-md">
        <div class="flex justify-center items-center">
            <span class="text-gray-300 font-semibold text-2xl">Create New User</span>
        </div>
        <form class="mt-4" method="post" action="?/register">
            <input type="text" name="username"  class="form-control mt-1 mb-3 bg-gray-800 text-white w-full focus:outline-none border-2 border-gray-700 rounded-lg px-4 py-2" placeholder="Enter your username"/>

            <input type="email" name="email" class="form-control mt-1 mb-3 bg-gray-800 text-white w-full focus:outline-none border-2 border-gray-700 rounded-lg px-4 py-2" placeholder="Enter your email address"/>

            <input type="password" name="password" class="form-control mt-1 bg-gray-800 text-white w-full focus:outline-none border-2 border-gray-700 rounded-lg px-4 py-2" placeholder="Enter your password"/>

            <div class="flex justify-between items-center mt-6">
                <div>
                    <label class="inline-flex items-center">
                        <input type="checkbox" class="form-checkbox text-blue-500 bg-gray-700">
                        <span class="mx-2 text-gray-400 text-sm">Remember Me</span>
                    </label>
                </div>

                <div>
                    <a href="#" class="block text-sm fontme text-blue-400 hover:underline">Forgot your password?</a>
                </div>
            </div>

            <div class="mt-6">
                <button type="submit" class="py-2 px-4 text-center bg-blue-600 rounded-md w-full text-white text-sm hover:bg-blue-500 focus:outline-none">
                    Create Account
                </button>
            </div>
        </form>
    </div>
</div>



window.clearAuthData = () => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("userEmail");
};

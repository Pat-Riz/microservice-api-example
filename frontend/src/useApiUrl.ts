export default function useApiUrl() {
  let platformURL: string;
  let commandURL: string;

  if (process.env.NODE_ENV === "development") {
    platformURL = "https://localhost:7070/platforms";
    commandURL = `https://localhost:7239/c/platforms/`;
  } else {
    platformURL = "https://patriz.com/platforms";
    commandURL = `https://localhost:7239/c/platforms/`;
  }
  return { platformURL, commandURL };
}

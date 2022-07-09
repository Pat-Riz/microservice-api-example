import axios from "axios";
import { useEffect, useState } from "react";
import Command from "./components/Command";
import CreateCommand from "./components/CreateCommand";
import CreatePlatform from "./components/CreatePlatform";
import NewButton from "./components/NewButton";
import Platform from "./components/Platform";
import useApiUrl from "./useApiUrl";

export interface IPlatform {
  id: number;
  name: string;
  publisher: string;
  cost: string;
}

export interface ICommand {
  id: number;
  platformId: number;
  howTo: string;
  commandLine: string;
}

function App() {
  const { platformURL, commandURL } = useApiUrl();
  const [platformId, setPlatformId] = useState<number>(0);
  const [platformName, setPlatformName] = useState<string>("");
  const [platforms, setPlatforms] = useState<IPlatform[]>([]);
  const [commands, setCommands] = useState<ICommand[]>([]);
  const [visibleNewCommand, setVisibleNewCommand] = useState<boolean>(false);
  const [visibleNewPlatform, setVisibleNewPlatform] = useState<boolean>(false);

  useEffect(() => {
    const getPlatforms = async () => {
      try {
        const res = await axios.get<IPlatform[]>(platformURL);
        setPlatforms(res.data);
      } catch (error) {
        console.log("Error:", error);
      }
    };
    document.body.style.background = "rgb(241 245 249 / 1)";
    getPlatforms();
  }, [platformURL]);

  useEffect(() => {
    const getCommands = async () => {
      try {
        const res = await axios.get<ICommand[]>(
          commandURL + `${platformId}/commands`
        );
        setCommands(res.data);
      } catch (error) {
        console.log("Error:", error);
      }
    };
    if (platformId > 0) getCommands();
  }, [commandURL, platformId]);

  const getCommands = (id: number): ICommand[] => {
    return commands.filter((c) => c.platformId === id);
  };

  const toggleVisibleCommand = () => {
    setVisibleNewCommand(!visibleNewCommand);
  };

  const toggleVisiblePlatform = () => {
    setVisibleNewPlatform(!visibleNewPlatform);
  };

  const blurEffect = visibleNewCommand || visibleNewPlatform ? "blur" : "";

  return (
    <div className='bg-slate-100'>
      <div className={`hidden sm:block container mx-auto px-4 ${blurEffect}`}>
        <h1 className='text-6xl mb-4 pt-12 font-extrabold animate-enter-right '>
          Platforms
        </h1>
        <NewButton clickFunction={toggleVisiblePlatform} />
        {platforms.map((platform) => {
          return (
            <Platform
              plat={platform}
              selectedId={platformId}
              setId={setPlatformId}
              setName={setPlatformName}
            />
          );
        })}
      </div>
      {platformId > 0 && (
        <div
          className={`container mx-auto px-4 animate-fadeIn-quick ${blurEffect}`}
        >
          <h2 className='text-4xl m-4'>{platformName} commands</h2>

          <NewButton clickFunction={toggleVisibleCommand} />
          {getCommands(platformId).map((command) => {
            return (
              <Command
                commandLine={command.commandLine}
                id={command.id}
                howTo={command.howTo}
              />
            );
          })}
        </div>
      )}
      {visibleNewPlatform && (
        <CreatePlatform cancelFunction={toggleVisiblePlatform} />
      )}
      {visibleNewCommand && (
        <CreateCommand
          cancelFunction={toggleVisibleCommand}
          platformId={platformId}
        />
      )}
    </div>
  );
}

export default App;

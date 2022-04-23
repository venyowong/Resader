package helper

import (
	"io"
	"log"
	"os"

	"github.com/leemcloughlin/logfile"
)

func InitLog() {
	logFile, err := logfile.New(
		&logfile.LogFile{
			FileName: "log.txt",
			MaxSize:  500 * 1024, // 500K duh!
			Flags:    logfile.FileOnly | logfile.OverWriteOnStart})
	if err != nil {
		log.Fatalf("Failed to create logFile: %s\n", err)
	}

	log.SetOutput(io.MultiWriter(os.Stdout, logFile))
}
